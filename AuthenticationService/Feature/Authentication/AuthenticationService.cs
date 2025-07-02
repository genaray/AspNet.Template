using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationService.Feature.Email;
using AuthenticationService.Feature.Frontend;
using AuthenticationService.Feature.UserCredentials;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared;

namespace AuthenticationService.Feature.Authentication;

public class AuthenticationService(
    ILogger<AuthenticateController> logger, 
    IConfiguration configuration, 
    IOptions<FrontendSettings> frontendSettings,
    UserManager<UserCredentials.UserCredentials> userManager, 
    RoleManager<IdentityRole> roleManager, 
    UserCredentialsService userCredentialsService,
    EmailService emailService
)
{
    /// <summary>
    /// Logs in <see cref="UserCredentials"/>.
    /// </summary>
    /// <param name="request">The <see cref="LoginRequest"/>.</param>
    /// <returns>An <see cref="LoginResponse"/> with an <see cref="UserCredentials"/> and details.</returns>
    /// <exception cref="UnauthorizedAccessException">Throws if the email or password are invalid.</exception>
    /// <exception cref="InvalidOperationException">Throws if the <see cref="UserCredentials"/> is not confirmed yet.</exception>
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        
        // Email or password is false
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return new LoginResponse { Result = new Result { Success = false, Error = new InvalidEmailOrPasswordException() } };   
        }

        // Email not confirmed
        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            return new LoginResponse { Result = new Result { Success = false, Error = new EmailNotConfirmedException() } };
        }
   
        // Everything okay, login
        var authClaims = await GetClaimsForUser(user);
        var token = GenerateJwtToken(authClaims);
        return new LoginResponse
        {
            Result = new Result { Success = true },
            
            User = user,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = token.ValidTo,
        };
    }
    
    /// <summary>
    /// Registers an <see cref="UserCredentials"/>.
    /// </summary>
    /// <param name="request">The <see cref="RegisterRequest"/>.</param>
    /// <returns>An <see cref="RegisterResponse"/> with an <see cref="UserCredentials"/>.</returns>
    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        // Registration of user
        var user = new UserCredentials.UserCredentials { UserName = request.Email, Email = request.Email };
        var response = await userCredentialsService.CreateUserCredentialsAsync(request.Username, request.Email, request.Password);
        
        return new RegisterResponse
        {
            Result = response.Result,
            User = user,
        };
    }

    /// <summary>
    /// Registers an <see cref="UserCredentials"/> as an admin.
    /// </summary>
    /// <param name="request">The <see cref="RegisterRequest"/>.</param>
    /// <returns>An <see cref="RegisterResponse"/> with an <see cref="UserCredentials"/>.</returns>
    public async Task<RegisterResponse> RegisterAdminAsync(RegisterRequest request)
    {
        // Register user
        var response = await RegisterAsync(request);
        if (!response.Result.Success)
        {
            return response;
        }
        
        // Make him admin
        await EnsureRolesExistAsync();
        await userManager.AddToRolesAsync(response.User!, [Roles.ADMIN.ToString(), Roles.USER.ToString()]);
        return response;
    }
    
    /// <summary>
    /// Confirms an email of an <see cref="UserCredentials"/>.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <param name="token">The token.</param>
    /// <returns>An <see cref="Task{TResult}"/> with an <see cref="Result"/>.</returns>
    public async Task<Result> ConfirmEmailAsync(string? email, string? token)
    {
        // Check if email and token are actually valid
        if (email == null || token == null)
        {
            return new Result { Success = false, Error = new InvalidLinkException(), };
        }

        // Check if user exists 
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new Result { Success = false, Error = new UserNotFoundException(), };
        }

        // Confirm email
        var result = await userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            return new Result { Success = true, };
        }

        // Otherwhise failed
        return new Result { Success = false, Error = new AppException(Error.EmailConfirmationFailed), };
    }

    /// <summary>
    /// Requests a password reset email for a certain <see cref="UserCredentials"/> by his email..
    /// </summary>
    /// <param name="request">The <see cref="RequestPasswordResetRequest"/>.</param>
    /// <returns>A <see cref="Task{TResult}"/> with an <see cref="Result"/>.</returns>
    public async Task<Result> RequestPasswordResetAsync(RequestPasswordResetRequest request)
    {
        // Find user
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new Result { Success = false, Error = new UserNotFoundException(), };
        }

        // Generate password reset token & link
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"{frontendSettings.Value.PasswordResetUrl}?email={Uri.EscapeDataString(request.Email)}&token={Uri.EscapeDataString(token)}";
        
        // Send Email
        await emailService.SendResetPasswordEmail(user.Email!, user.UserName!, resetLink!);
        return new Result { Success = true, };
    }

    /// <summary>
    /// Resets a password for a certain <see cref="UserCredentials"/>.
    /// </summary>
    /// <param name="request">The <see cref="ResetPasswordRequest"/>.</param>
    /// <returns>A <see cref="Task{TResult}"/> with an <see cref="Result"/>.</returns>
    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        // Find user
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new Result { Success = false, Error = new UserNotFoundException() };
        }

        // Reset/change password
        var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (result.Succeeded) return new Result{ Success = true, };
        
        // Collect errors and send BadRequest
        var errors = result.Errors.Select(e => e.Description).ToList();
        return new Result { Success = false, Error = new PasswordResetFailedException(Error.PasswordResetFailed, errors) };
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
    /// Returns the JWT-Claims for an <see cref="UserCredentials"/>.
    /// </summary>
    /// <param name="userCredentials">The <see cref="UserCredentials"/>.</param>
    /// <returns>An <see cref="Task{TResult}"/> with a <see cref="List{T}"/> of <see cref="Claim"/>s.</returns>
    private async Task<List<Claim>> GetClaimsForUser(UserCredentials.UserCredentials userCredentials)
    {
        var userRoles = await userManager.GetRolesAsync(userCredentials);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userCredentials.UserName),
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