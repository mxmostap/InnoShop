using UserManagement.Domain.Entities;

namespace UserManagement.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(User user, string resetToken);
    Task SendEmailConfirmationAsync(User user, string confirmationToken);
}