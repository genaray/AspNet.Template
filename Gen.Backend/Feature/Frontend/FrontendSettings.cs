namespace Gen.Backend.Feature.Frontend;

/// <summary>
/// The <see cref="FrontendSettings"/> class
/// contains all set settings to reference websites in the frontend. 
/// </summary>
public class FrontendSettings
{
    public required string Url { get; set; }
    public required string ConfirmEmail { get; set; }
    public required string RequestPasswordReset { get; set; }
    public required string PasswordReset { get; set; }
    
    public string ConfirmEmailUrl => $"{Url}/{ConfirmEmail}";
    public string RequestPasswordResetUrl => $"{Url}/{RequestPasswordReset}";
    public string PasswordResetUrl => $"{Url}/{PasswordReset}";
}