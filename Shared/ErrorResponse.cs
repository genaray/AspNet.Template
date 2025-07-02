namespace Shared;

public class ErrorResponse
{
    public string Message { get; set; } = null!;
    public List<string> Details { get; set; } = null!;
}