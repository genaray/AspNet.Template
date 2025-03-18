using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace Gen.Backend.Feature.Email;

/// <summary>
/// The <see cref="EmailSender"/> class
/// manages the connection to the email server and has methods for sending emails.
/// </summary>
/// <param name="emailSettings">The <see cref="EmailSettings"/>.</param>
public class EmailSender(ILogger<EmailSender> _logger, IOptions<EmailSettings> emailSettings) : IEmailSender
{
    /// <summary>
    /// Sends an email with a subject and html async.
    /// <remarks>Currently opens and closes a connection EACH TIME.</remarks>
    /// </summary>
    /// <param name="email">The email-address.</param>
    /// <param name="subject">The subject.</param>
    /// <param name="htmlMessage">The html.</param>
    /// <returns>A <see cref="Task"/> to track.</returns>
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var settings = emailSettings.Value;
        using var client = new SmtpClient(settings.SmtpServer, settings.Port);
        client.Credentials = new NetworkCredential(settings.Username, settings.Password);
        client.EnableSsl = settings.EnableSsl;
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.UseDefaultCredentials = false;

        var mailMessage = new MailMessage
        {
            From = new MailAddress(settings.SenderEmail, settings.SenderDisplayName),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        mailMessage.To.Add(email);
        try
        {
            await client.SendMailAsync(mailMessage); // Wait till send is done
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}