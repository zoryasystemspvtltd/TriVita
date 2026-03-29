using CommunicationService.Application.Abstractions;
using CommunicationService.Application.Models;
using CommunicationService.Application.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CommunicationService.Infrastructure.Services;

public sealed class EmailService : IEmailService
{
    private readonly CommunicationOptions _options;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<CommunicationOptions> options, ILogger<EmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ChannelSendResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (message.ToAddresses.Count == 0)
            return ChannelSendResult.Fail("No email recipients.");

        var smtp = _options.Smtp;
        try
        {
            using var client = new SmtpClient();
            var secure = smtp.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
            await client.ConnectAsync(smtp.Host, smtp.Port, secure, cancellationToken);

            if (!string.IsNullOrWhiteSpace(smtp.UserName))
                await client.AuthenticateAsync(smtp.UserName, smtp.Password ?? string.Empty, cancellationToken);

            var mime = new MimeMessage();
            mime.From.Add(new MailboxAddress(smtp.FromName, smtp.FromAddress));
            foreach (var to in message.ToAddresses)
                mime.To.Add(MailboxAddress.Parse(to));

            mime.Subject = message.Subject;
            var builder = new BodyBuilder();
            if (message.IsHtml)
                builder.HtmlBody = message.Body;
            else
                builder.TextBody = message.Body;

            mime.Body = builder.ToMessageBody();

            await client.SendAsync(mime, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            return ChannelSendResult.Ok("sent");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMTP send failed.");
            return ChannelSendResult.Fail(ex.Message);
        }
    }
}
