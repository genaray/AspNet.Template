using Microsoft.AspNetCore.Mvc;
using Shared;

namespace UserService.Feature;

public static class AppExceptionExtensions
{
    public static IActionResult ToIActionResult(this AppException ex)
    {
        return ex switch
        {
            // Auth
            UserAlreadyExistsException e => new ConflictObjectResult(e.Message),
            UserNotFoundException e => new NotFoundObjectResult(e.Message),
        };
    }
}


public class UserAlreadyExistsException : AppException
{
    public UserAlreadyExistsException(string message = Error.UserAlreadyExists) : base(message) { }
}

public class UserNotFoundException : AppException
{
    public UserNotFoundException(string message = Error.UserNotFound) : base(message) { }
}
