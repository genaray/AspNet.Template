namespace Gen.Backend.Feature.Email;

/// <summary>
/// The <see cref="EmailSettings"/> class
/// contains all set settings to establish an email-server connection. 
/// </summary>
public class EmailSettings
{
    public required string SmtpServer { get; set; }
    public required int Port { get; set; }
    public required string SenderEmail { get; set; }
    public required string SenderName { get; set; }
    public required string Password { get; set; }
    public required bool EnableSsl { get; set; }
}