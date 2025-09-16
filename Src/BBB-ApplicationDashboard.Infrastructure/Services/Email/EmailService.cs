using BBB_ApplicationDashboard.Application.DTOs;
using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BBB_ApplicationDashboard.Infrastructure.Services.Email;

public class EmailService : IEmailService
{
    private readonly EmailOptions emailOptions;
    private readonly ILogger<EmailService> logger;

    public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger)
    {
        this.emailOptions = options.Value;
        this.logger = logger;
    }

    public async Task<bool> SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        //! 1️⃣ initialize mail
        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(emailOptions.FROM_NAME, emailOptions.FROM_EMAIL));
        mime.To.Add(MailboxAddress.Parse(message.To));
        //! 4️⃣ add email Subject
        mime.Subject = message.Subject;

        //! 5️⃣ add HTML body
        var body = new BodyBuilder { HtmlBody = message.HtmlBody };
        mime.Body = body.ToMessageBody();

        try
        {
            using var client = new SmtpClient { CheckCertificateRevocation = true };
            //! 6️⃣ email port and host (might cause problems)
            await client.ConnectAsync(
                emailOptions.SMTP_HOST,
                emailOptions.SMTP_PORT,
                SecureSocketOptions.StartTls,
                cancellationToken
            );
            await client.AuthenticateAsync(
                emailOptions.SMTP_USERNAME,
                emailOptions.SMTP_PASSWORD,
                cancellationToken
            );

            await client.SendAsync(mime, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "💥 Failed to send email to {To}", message.To);
            return false;
        }
    }
}
