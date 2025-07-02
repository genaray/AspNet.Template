namespace Shared;

/// <summary>
/// The <see cref="AppException"/> class
/// represents an exception with a basic message. 
/// </summary>
public class AppException : Exception
{
    public AppException(string message) : base(message) { }
}