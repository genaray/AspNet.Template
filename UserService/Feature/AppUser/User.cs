using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Shared;

namespace UserService.Feature.AppUser;

/// <summary>
/// The <see cref="RegisterUserRequest"/> struct
/// Is a request that is sent to the associated controller at register with all register-specific data.
/// </summary>
public struct RegisterUserRequest
{
    [Required(ErrorMessage = "User Name is required")]
    public string Username { get; set; }
    
    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "FirstName is required")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "LastName is required")]
    public string LastName { get; set; }
}

/// <summary>
/// The <see cref="RegisterUserResponse"/> struct
/// Is a response that is returned after a <see cref="RegisterUserRequest"/>. 
/// </summary>
public struct RegisterUserResponse
{
    public required Result Result { get; set; }
    
    public User? User { get; set; }
}

/// <summary>
/// The <see cref="CreateOrUpdateUserDto"/> struct
/// contains all required data to create or update an <see cref="User"/>.
/// </summary>
public struct CreateOrUpdateUserDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

/// <summary>
/// The <see cref="UserDto"/> struct
/// represents an <see cref="User"/> with all his transferable data.
/// </summary>
public struct UserDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

/// <summary>
/// The <see cref="User"/> struct
/// acts as an user and contains all his service relevant data.
/// </summary>
public class User
{
    public string Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}