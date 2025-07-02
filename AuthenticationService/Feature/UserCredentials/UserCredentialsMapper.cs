using Riok.Mapperly.Abstractions;

namespace AuthenticationService.Feature.UserCredentials;
#pragma warning disable RMG020

/// <summary>
/// The <see cref="UserCredentialsMapper"/> class
/// defines a mapper that automatically generates code to convert and update between the <see cref="UserCredentials"/> and his Dtos.
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public static partial class UserCredentialsMapper
{
    /// <summary>
    /// Converts an <see cref="UserCredentials"/> to its <see cref="UserCredentialsDto"/>.
    /// </summary>
    /// <param name="userCredentials">The <see cref="UserCredentials"/>.</param>
    /// <returns>The converted <see cref="UserCredentialsDto"/>.</returns>
    [MapProperty(nameof(UserCredentials.Id), nameof(UserCredentialsDto.Id))]
    [MapProperty(nameof(UserCredentials.UserName), nameof(UserCredentialsDto.Username))]
    public static partial UserCredentialsDto ToDto(this UserCredentials userCredentials);
    
    /// <summary>
    /// Converts an <see cref="IEnumerable{T}"/> of <see cref="UserCredentials"/>s to an <see cref="IEnumerable{T}"/> of <see cref="UserCredentialsDto"/>s.
    /// </summary>
    /// <param name="user">The <see cref="IEnumerable{T}"/> of <see cref="UserCredentials"/>s.</param>
    /// <returns>The <see cref="IEnumerable{T}"/> of <see cref="UserCredentialsDto"/>s.</returns>
    public static partial IEnumerable<UserCredentialsDto> ToDtoList(this IEnumerable<UserCredentials> user);
    
    /// <summary>
    /// Converts an <see cref="UserCredentialsDto"/> to its <see cref="UserCredentials"/>.
    /// </summary>
    /// <param name="credentialsDto">The <see cref="UserCredentialsDto"/>.</param>
    /// <returns>The converted <see cref="UserCredentials"/>.</returns>
    [MapperIgnoreTarget(nameof(UserCredentials.Id))]
    [MapProperty(nameof(CreateOrUpdateUserCredentialsDto.Username), nameof(UserCredentials.UserName))]
    [MapProperty(nameof(CreateOrUpdateUserCredentialsDto.Password), nameof(UserCredentials.PasswordHash))]
    public static partial UserCredentials ToEntity(this CreateOrUpdateUserCredentialsDto credentialsDto);
    
    /// <summary>
    /// Merges a <see cref="CreateOrUpdateUserCredentialsDto"/> into an <see cref="UserCredentials"/>.
    /// </summary>
    /// <param name="userCredentials">The <see cref="UserCredentials"/>.</param>
    /// <param name="userCredentialsDto">The <see cref="CreateOrUpdateUserCredentialsDto"/>.</param>
    [MapperIgnoreTarget(nameof(UserCredentials.Id))]
    [MapProperty(nameof(CreateOrUpdateUserCredentialsDto.Username), nameof(UserCredentials.UserName))]
    [MapProperty(nameof(CreateOrUpdateUserCredentialsDto.Password), nameof(UserCredentials.PasswordHash))]
    public static partial void MergeWith([MappingTarget] this UserCredentials userCredentials, CreateOrUpdateUserCredentialsDto userCredentialsDto);
}

#pragma warning restore RMG020