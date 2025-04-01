using System.Net;
using System.Net.Mail;
using Platform.Application.InfrastructureInterfaces;
using Serilog;

namespace Platform.Infrastructure.Gateways.Smtp;

public class SmtpGateway : IEmailGateway
{
    private readonly SmtpClient _smtpClient;
    private readonly SmtpSettings _smtpSettings;

    public SmtpGateway(SmtpSettings smtpSettings)
    {
        _smtpClient = new SmtpClient()
        {
            Host = smtpSettings.Host,
            Port = smtpSettings.Port,
            Credentials = new NetworkCredential(smtpSettings.Login, smtpSettings.Password),
            EnableSsl = true,
        };

        _smtpSettings = smtpSettings;
    }

    public async Task SendEmailAsync(string subject, string body, string sendTo, CancellationToken cancellationToken)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
            ReplyToList = { new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName) },
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };
        
        mailMessage.To.Add(sendTo);
    
        try
        {
            await _smtpClient.SendMailAsync(mailMessage, cancellationToken);
            Log.Information("Email send successfully");
        }
        catch (Exception ex)
        {
            Log.Information("Failure during sending email: {Message}", ex.Message);
        }
    }
}
