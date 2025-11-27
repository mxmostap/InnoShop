namespace UserManagement.Application.Common.Interfaces;

public interface ISmtpClient
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}