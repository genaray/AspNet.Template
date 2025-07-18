namespace AspNet.Backend.Feature.Email;

public struct RegistrationTemplate
{
    public string Name { get; set; }
    public string ConfirmationLink { get; set; }
}

public struct PasswordResetTemplate
{
    public string Name { get; set; }
    public string ResetLink { get; set; }
}

/// <summary>
/// The <see cref="EmailService"/> class
/// manages the sending of email templates using the <see cref="EmailSender"/> and the <see cref="EmailTemplateRenderer"/>.  
/// </summary>
/// <param name="sender">The <see cref="EmailSender"/>.</param>
/// <param name="templateRenderer">The <see cref="EmailTemplateRenderer"/>.</param>
public class EmailService(EmailSender sender, EmailTemplateRenderer templateRenderer)
{
    /// <summary>
    /// Sends an Email to confirm the email and user account.
    /// </summary>
    /// <param name="email">The email-address-</param>
    /// <param name="username">The username.</param>
    /// <param name="confirmationLink">The link to confirm the email.</param>
    public async Task SendConfirmEmail(string email, string username, string confirmationLink)
    {
        var emailBody = await templateRenderer.RenderTemplateAsync("Email/RegistrationEmail", new RegistrationTemplate{ Name = username, ConfirmationLink = confirmationLink});
        await sender.SendEmailAsync(email, $"Welcome {username} - Confirm your Email", emailBody);
    }
    
    /// <summary>
    /// Sends an Email to reset the password for an user.
    /// </summary>
    /// <param name="email">The email-address-</param>
    /// <param name="username">The username.</param>
    /// <param name="resetLink">The link to reset the password.</param>
    public async Task SendResetPasswordEmail(string email, string username, string resetLink)
    {
        var emailBody = await templateRenderer.RenderTemplateAsync("Email/PasswordResetEmail", new PasswordResetTemplate{ Name = username, ResetLink = resetLink});
        await sender.SendEmailAsync(email, $"Reset your password", emailBody);
    }
}