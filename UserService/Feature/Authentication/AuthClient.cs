using System.Net;
using Shared;

namespace UserService.Feature.Authentication;

public class RegisterCredentialsResponse
{
    public string Message { get; set; } = null!;
    public string UserId  { get; set; } = null!;
}

public class RegisterResponse
{
    public Result Result { get; set; }
    public string UserId  { get; set; } = null!;
}

public class GetUserCredentialsByIdResponse
{
    public string Id { get; set; } = null!;
}

public interface IAuthClient
{
    Task<RegisterResponse> RegisterUserCredentialsAsync(string username, string email, string password);

    Task<RegisterResponse> GetUserCredentialsIdByEmail(string email);
}

public class AuthClient : IAuthClient
{
    private readonly HttpClient _http;

    public AuthClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<RegisterResponse> RegisterUserCredentialsAsync(string username, string email, string password)
    {
        // DTO, das im AuthService erwartet wird
        var dto = new
        {
            Username = username,
            Email    = email,
            Password = password,
        };

        // Call auth service
        var response = await _http.PostAsJsonAsync("/api/Authenticate/register", dto);

        // Already exists
        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            var errorPayload = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            return new RegisterResponse
            {
                Result = new Result{ Success = false, Error = new UserAlreadyExistsException(errorPayload!.Message)},
            };
        }

        response.EnsureSuccessStatusCode();

        // Deserialize response
        var deserializedResponse = await response.Content.ReadFromJsonAsync<RegisterCredentialsResponse>();
        return new RegisterResponse
        {
            Result = new Result{ Success = true }, 
            UserId = deserializedResponse!.UserId
        };
    }
    
    public async Task<RegisterResponse> GetUserCredentialsIdByEmail(string email)
    {
        // Call auth service
        var response = await _http.GetAsync($"/api/Authenticate/{email}");

        // Already exists
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new RegisterResponse
            {
                Result = new Result{ Success = false, Error = new UserNotFoundException()},
            };
        }

        response.EnsureSuccessStatusCode();

        // Deserialize response
        var deserializedResponse = await response.Content.ReadFromJsonAsync<GetUserCredentialsByIdResponse>();
        return new RegisterResponse
        {
            Result = new Result{ Success = true }, 
            UserId = deserializedResponse!.Id
        };
    }
}