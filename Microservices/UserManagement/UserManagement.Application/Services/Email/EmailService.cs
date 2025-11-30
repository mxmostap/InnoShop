using Microsoft.Extensions.Options;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Services.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ISmtpClient _smtpClient;

    public EmailService(IOptions<EmailSettings> emailSettings, ISmtpClient smtpClient)
    {
        _emailSettings = emailSettings.Value;
        _smtpClient = smtpClient;
    }
    
    
    public async Task SendPasswordResetEmailAsync(User user, string resetToken)
    {
        var resetLink = $"{_emailSettings.BaseUrl}/reset-password-confirm?Email={user.Email}&Token={resetToken}";
        var subject = "Восстановление пароля";
        var body = $@"
                Здравствуйте, {user.Profile.FirstName} {user.Profile.LastName}!

                Для восстановления вашего пароля перейдите по ссылке:
                {resetLink}

                Ссылка действительна в течении {_emailSettings.TokenExpirationHours} часов.

                Если вы не запрашивали сброс пароля, проигнорируйте это письмо.";

        await _smtpClient.SendEmailAsync(user.Email, subject, body);
    }

    public async Task SendEmailConfirmationAsync(User user, string confirmationToken)
    {
        var confirmationLink = $"{_emailSettings.BaseUrl}confirm-email?Email={user.Email}&Token={confirmationToken}";
            
        var subject = "Подтверждение email адреса";
        var body = $@"
                Здравствуйте, {user.Profile.FirstName} {user.Profile.LastName}!
                
                Добро пожаловать в наш сервис! Для завершения регистрации 
                подтвердите ваш email адрес, перейдя по ссылке:
                {confirmationLink}
                
                Ссылка действительна в течение {_emailSettings.TokenExpirationHours} часов.";

        await _smtpClient.SendEmailAsync(user.Email, subject, body);
    }
}