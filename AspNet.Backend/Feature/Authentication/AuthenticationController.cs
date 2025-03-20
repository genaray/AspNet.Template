using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AspNet.Backend.Feature.AppUser;
using AspNet.Backend.Feature.Email;
using AspNet.Backend.Feature.Frontend;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AspNet.Backend.Feature.Authentication;

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
    AuthenticationService authenticationService
) : ControllerBase {
    
    /// <summary>
    /// Logs in <see cref="User"/>.
    /// </summary>
    /// <param name="request">The <see cref="LoginRequest"/>.</param>
    /// <returns>An <see cref="Task{TResult}"/> with an <see cref="IActionResult"/>.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Login
        var response = await authenticationService.LoginAsync(request);
        return response.Result.Error != null ? response.Result.Error.ToIActionResult() : Ok(new {response.Token, response.Expiration});
    }
    
    /// <summary>
    /// Registers an <see cref="User"/>.
    /// </summary>
    /// <param name="request">The <see cref="RegisterRequest"/>.</param>
    /// <returns>An <see cref="Task{TResult}"/> with an <see cref="IActionResult"/>.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // Register
        var response = await authenticationService.RegisterAsync(request);
        return response.Result.Error != null ? response.Result.Error.ToIActionResult() : Ok(Success.UserCreated);
    }
    
    /// <summary>
    /// Registers an admin <see cref="User"/>.
    /// </summary>
    /// <param name="request">The <see cref="RegisterRequest"/>.</param>
    /// <returns>An <see cref="Task{TResult}"/> with an <see cref="IActionResult"/>.</returns>
    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest request)
    {
        // Register
        var response = await authenticationService.RegisterAdminAsync(request);
        return response.Result.Error != null ? response.Result.Error.ToIActionResult() : Ok(Success.UserCreated);
    }
    
    /// <summary>
    /// Confirms the email of an <see cref="User"/>.
    /// </summary>
    /// <param name="email">The <see cref="IdentityUser{string}.Email"/>.</param>
    /// <param name="token">The confirmation token.</param>
    /// <returns>An <see cref="Task{TResult}"/> with the <see cref="ActionResult"/>.</returns>
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string? email, string? token)
    {
        var result = await authenticationService.ConfirmEmailAsync(email, token);
        return result.Error != null ? result.Error.ToIActionResult() : Ok(Success.EmailConfirmed);
    }
    
    /// <summary>
    /// Sends an email for changing the password.
    /// </summary>
    /// <param name="request">The <see cref="RequestPasswordResetRequest"/>.</param>
    /// <returns>An <see cref="Task{TResult}"/> with the <see cref="ActionResult"/>.</returns>
    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetRequest request)
    {
        // Request email
        var result = await authenticationService.RequestPasswordResetAsync(request);
        return result.Error != null ? result.Error.ToIActionResult() : Ok();
    }
    
    /// <summary>
    /// Confirm a password change.
    /// </summary>
    /// <param name="request">The <see cref="ResetPasswordRequest"/>.</param>
    /// <returns>An <see cref="Task{TResult}"/> with the <see cref="ActionResult"/>.</returns>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await authenticationService.ResetPasswordAsync(request);
        return result.Error != null ? result.Error.ToIActionResult() : Ok(Success.PasswordReset);
    }
}