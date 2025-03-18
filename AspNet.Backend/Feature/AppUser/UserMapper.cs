using System.Diagnostics.CodeAnalysis;
using Riok.Mapperly.Abstractions;

namespace Gen.Backend.Feature.AppUser;
#pragma warning disable RMG020

/// <summary>
/// The <see cref="UserMapper"/> class
/// defines a mapper that automatically generates code to convert and update between the <see cref="User"/> and his Dtos.
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public static partial class UserMapper
{
    
    /// <summary>
    /// Converts an <see cref="User"/> to its <see cref="UserDto"/>.
    /// </summary>
    /// <param name="user">The <see cref="User"/>.</param>
    /// <returns>The converted <see cref="UserDto"/>.</returns>
    [MapProperty(nameof(User.Id), nameof(UserDto.Id))]
    [MapProperty(nameof(User.UserName), nameof(UserDto.Username))]
    public static partial UserDto ToDto(this User user);
    
    /// <summary>
    /// Converts an <see cref="IEnumerable{T}"/> of <see cref="User"/>s to an <see cref="IEnumerable{T}"/> of <see cref="UserDto"/>s.
    /// </summary>
    /// <param name="user">The <see cref="IEnumerable{T}"/> of <see cref="User"/>s.</param>
    /// <returns>The <see cref="IEnumerable{T}"/> of <see cref="UserDto"/>s.</returns>
    public static partial IEnumerable<UserDto> ToDtoList(this IEnumerable<User> user);
    
    /// <summary>
    /// Converts an <see cref="UserDto"/> to its <see cref="User"/>.
    /// </summary>
    /// <param name="dto">The <see cref="UserDto"/>.</param>
    /// <returns>The converted <see cref="User"/>.</returns>
    [MapperIgnoreTarget(nameof(User.Id))]
    [MapProperty(nameof(CreateOrUpdateUserDto.Username), nameof(User.UserName))]
    [MapProperty(nameof(CreateOrUpdateUserDto.Password), nameof(User.PasswordHash))]
    public static partial User ToEntity(this CreateOrUpdateUserDto dto);
    
    /// <summary>
    /// Merges a <see cref="CreateOrUpdateUserDto"/> into an <see cref="User"/>.
    /// </summary>
    /// <param name="user">The <see cref="User"/>.</param>
    /// <param name="userDto">The <see cref="CreateOrUpdateUserDto"/>.</param>
    [MapperIgnoreTarget(nameof(User.Id))]
    [MapProperty(nameof(CreateOrUpdateUserDto.Username), nameof(User.UserName))]
    [MapProperty(nameof(CreateOrUpdateUserDto.Password), nameof(User.PasswordHash))]
    public static partial void MergeWith([MappingTarget] this User user, CreateOrUpdateUserDto userDto);
}

#pragma warning restore RMG020