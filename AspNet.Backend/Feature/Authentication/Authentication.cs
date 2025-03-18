using System.ComponentModel.DataAnnotations;

namespace Gen.Backend.Feature.Authentication;

/// <summary>
/// The <see cref="LoginRequest"/> struct
/// Is a request that is sent to the associated controller at login with all login-specific data.
/// </summary>
public struct LoginRequest
{
    [EmailAddress]
    [Required(ErrorMessage = "Email required")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}

/// <summary>
/// The <see cref="RegisterRequest"/> struct
/// Is a request that is sent to the associated controller at register with all register-specific data.
/// </summary>
public struct RegisterRequest
{
    [Required(ErrorMessage = "User Name is required")]
    public string Username { get; set; }
    
    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}

/// <summary>
/// The <see cref="RequestPasswordResetRequest"/> struct
/// Is a request that is sent to the associated controller when the <see cref="AppUser.User"/> forgot his password with his email included.
/// </summary>
public struct RequestPasswordResetRequest
{
    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
}

/// <summary>
/// The <see cref="ResetPasswordRequest"/> struct
/// Is a request that is sent to the associated controller when the <see cref="AppUser.User"/> resets his password with the token, email and new password included.
/// </summary>
public struct ResetPasswordRequest
{
    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Token is required")]
    public string Token { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string NewPassword { get; set; }
}