using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using UserManagement.Application.Common.Interfaces;

namespace UserManagement.Application.Services.Email;

public class SmtpClient : ISmtpClient, IDisposable
{
    private readonly System.Net.Mail.SmtpClient _smtpClient;
    private readonly SmtpSettings _smtpSettings;

    public SmtpClient(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;

        _smtpClient = new System.Net.Mail.SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
        {
            Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
            EnableSsl = _smtpSettings.EnableSsl
        };
    }
    
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        
        mailMessage.To.Add(toEmail);

        await _smtpClient.SendMailAsync(mailMessage);
    }

    public void Dispose()
    {
        _smtpClient?.Dispose();
    }
}