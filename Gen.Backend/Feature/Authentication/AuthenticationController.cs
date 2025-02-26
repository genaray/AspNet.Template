using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Gen.Backend.Feature.AppUser;
using Gen.Backend.Feature.Email;

namespace Gen.Backend.Feature.Authentication;

/// <summary>
/// The <see cref="AuthenticateController"/> class
/// is a controller managing all authentication related tasks like login, registration, password reset or email confirmation. 
/// </summary>
/// <param name="logger">The <see cref="ILogger{TCategoryName}"/> instance.</param>
/// <param name="configuration">The <see cref="IConfiguration"/>.</param>
/// <param name="userManager">The <see cref="UserManager{TUser}"/>.</param>
/// <param name="roleManager">The <see cref="RoleManager{TCategoryName}"/>.</param>
/// <param name="emailService">The <see cref="EmailService"/>.</param>
[Route("api/[controller]")]
[ApiController]
public class AuthenticateController(
    ILogger<AuthenticateController> logger, 
    IConfiguration configuration, 
    UserManager<User> userManager, 
    RoleManager<IdentityRole> roleManager, 
    EmailService emailService
) : ControllerBase {
    
    /// <summary>
    /// Logs in <see cref="User"/>.
    /// </summary>
    /// <param name="request">The <see cref="LoginRequest"/>.</param>
    /// <returns>An <see cref="Task{TResult}"/> with an <see cref="IActionResult"/>.</returns>
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        
        logger.LogInformation("Login request");
        
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password)) return Unauthorized(Error.InvalidEmailOrPassword);

        // Force user to confirm email first
        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            return BadRequest(Error.EmailNotConfirmed);
        }
        
        // Set Role and jwt token
        var authClaims = await GetClaimsForUser(user);
        var token = GenerateJwtToken(authClaims);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        });
    }
    
    /// <summary>
    /// Registers an <see cref="User"/>.
    /// </summary>
    /// <param name="request">The <see cref="RegisterRequest"/>.</param>
    /// <returns>An <see cref="Task{TResult}"/> with an <see cref="IActionResult"/>.</returns>
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await CreateUserAsync(request);
        return result.User != null ? Ok(Success.UserCreated) : BadRequest(new { result.Message, result.Details });
    }
    
    /// <summary>
    /// Registers an admin <see cref="User"/>.
    /// </summary>
    /// <param name="request">The <see cref="RegisterRequest"/>.</param>
    /// <returns>An <see cref="Task{TResult}"/> with an <see cref="IActionResult"/>.</returns>
    [HttpPost]
    [Route("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest request)
    {
        var result = await CreateUserAsync(request);
        if (result.User == null) return BadRequest(new { result.Message, result.Details });

        // Make user admin
        await EnsureRolesExistAsync();
        await userManager.AddToRolesAsync(result.User, [Roles.ADMIN.ToString(), Roles.USER.ToString()]);

        return Ok(Success.UserCreated);
    }
    
    /// <summary>
    /// Creates an <see cref="User"/>.
    /// </summary>
    /// <param name="request">The <see cref="RegisterRequest"/>.</param>
    /// <returns>An <see cref="Task{TResult}"/> with User-Data indicating whether the User was created or not.</returns>
    private async Task<(User? User, string Message, List<string> Details)> CreateUserAsync(RegisterRequest request)
    {
        if (await userManager.FindByNameAsync(request.Username) != null) return (null, Error.UserAlreadyExists, []);

        var user = new User
        {
            Email = request.Email,
            UserName = request.Username,
            SecurityStamp = Guid.NewGuid().ToString(),
            RegisterDate = DateTime.Now,
        };

        // Register 
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return (null, Error.UserCreationFailed,result.Errors.Select(e => e.Description).ToList());
        
        // Send Confirmation-Email
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = Url.Action("ConfirmEmail", "Authenticate", new { userId = user.Id, token }, Request.Scheme);

        await emailService.SendConfirmEmail(user.Email, user.UserName, confirmationLink!);
        return (user, Success.UserCreated, []);
    }
    
    /// <summary>
    /// Confirms the email of an <see cref="User"/>.
    /// </summary>
    /// <param name="userId">The <see cref="User.Id"/>.</param>
    /// <param name="token">The confirmation token.</param>
    /// <returns>An <see cref="Task{TResult}"/> with the <see cref="ActionResult"/>.</returns>
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string? userId, string? token)
    {
        if (userId == null || token == null)
        {
            return BadRequest(Error.InvalidLink);
        }

        // Check if user exists 
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(Error.UserNotFound);
        }

        // Confirm email
        var result = await userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            return Ok(Success.EmailConfirmed);
        }

        return BadRequest(Error.EmailConfirmationFailed);
    }
    
    /// <summary>
    /// Sends an email for changing the password.
    /// </summary>
    /// <param name="request">The <see cref="ForgotPasswordRequest"/>.</param>
    /// <returns>An <see cref="Task{TResult}"/> with the <see cref="ActionResult"/>.</returns>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return NotFound(Error.UserNotFound);
        }

        // Generate password reset token & link
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = Url.Action("ResetPassword", "Authenticate", new { token, email = user.Email }, Request.Scheme);

        // Send Email
        await emailService.SendResetPasswordEmail(user.Email!, user.UserName!, resetLink!);
        return Ok();
    }
    
    /// <summary>
    /// Confirm a password change.
    /// </summary>
    /// <param name="request">The <see cref="ResetPasswordRequest"/>.</param>
    /// <returns>An <see cref="Task{TResult}"/> with the <see cref="ActionResult"/>.</returns>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return BadRequest(Error.PasswordResetFailed);
        }

        // Reset/change password
        var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(Error.PasswordResetFailed);
        }

        return Ok(Success.PasswordReset);
    }


    /// <summary>
    /// Confirms all possible <see cref="Roles"/> exist.
    /// </summary>
    /// <returns>An <see cref="Task"/>.</returns>
    private async Task EnsureRolesExistAsync()
    {
        if (!await roleManager.RoleExistsAsync(Roles.ADMIN.ToString()))
            await roleManager.CreateAsync(new IdentityRole(Roles.ADMIN.ToString()));
        if (!await roleManager.RoleExistsAsync(Roles.USER.ToString()))
            await roleManager.CreateAsync(new IdentityRole(Roles.USER.ToString()));
    }

    /// <summary>
    /// Returns the JWT-Claims for an <see cref="User"/>.
    /// </summary>
    /// <param name="user">The <see cref="User"/>.</param>
    /// <returns>An <see cref="Task{TResult}"/> with a <see cref="List{T}"/> of <see cref="Claim"/>s.</returns>
    private async Task<List<Claim>> GetClaimsForUser(User user)
    {
        var userRoles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
        return claims;
    }

    /// <summary>
    /// Generates an <see cref="JwtSecurityToken"/> based on the passed <see cref="Claim"/>s.
    /// </summary>
    /// <param name="claims">The <see cref="IEnumerable{T}"/> of <see cref="Claim"/>s.</param>
    /// <returns>An <see cref="JwtSecurityToken"/>.</returns>
    private JwtSecurityToken GenerateJwtToken(IEnumerable<Claim> claims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

        return new JwtSecurityToken(
            issuer: configuration["JWT:ValidIssuer"],
            audience: configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: claims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
    }
}