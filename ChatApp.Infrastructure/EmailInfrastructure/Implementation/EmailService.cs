using System.Diagnostics.CodeAnalysis;
using ChatApp.Infrastructure.configurations;
using ChatApp.Infrastructure.EmailInfrastructure.Interface;
using ChatApp.Infrastructure.models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ChatApp.Infrastructure.EmailInfrastructure.Implementation;

[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
    {
        _emailSettings = options.Value ?? throw new ArgumentNullException(nameof(options));

        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(EmailDetails emailDetails)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmailAddress));

        message.To.Add(new MailboxAddress("", emailDetails.RecipientEmail));

        message.Subject = emailDetails.Subject;

        message.Body = new TextPart("html")
        {
            Text = emailDetails.Body
        };

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port,
                SecureSocketOptions.SslOnConnect);

            await client.AuthenticateAsync(_emailSettings.SenderEmailAddress, _emailSettings.EmailPassword);

            await client.SendAsync(message);

            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {email}", emailDetails.RecipientEmail);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending email");

            return false;
        }
    }
}