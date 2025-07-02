using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Feature.UserCredentials;

/// <summary>
/// The <see cref="CreateOrUpdateUserCredentialsDto"/> struct
/// contains all required data to create or update an <see cref="UserCredentials"/>.
/// </summary>
public struct CreateOrUpdateUserCredentialsDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

/// <summary>
/// The <see cref="UserCredentialsDto"/> struct
/// represents an <see cref="UserCredentials"/> with all his transferable data.
/// </summary>
public struct UserCredentialsDto
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTimeOffset RegisterDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
}

/// <summary>
/// The <see cref="UserCredentials"/> struct
/// is based on <see cref="IdentityUser"/>, acts as an user and contains all his data.
/// </summary>
public class UserCredentials : IdentityUser
{
    public DateTime RegisterDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
}