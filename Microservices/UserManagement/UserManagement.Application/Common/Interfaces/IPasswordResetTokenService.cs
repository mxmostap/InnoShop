using UserManagement.Domain.Entities;

namespace UserManagement.Application.Common.Interfaces;

public interface IPasswordResetTokenService
{
    Task<string> GenerateAndSaveResetTokenAsync(int userId, TimeSpan expiration);
    Task<bool> ValidateResetTokenAsync(int userId, string token);
    Task InvalidateResetTokenAsync(int userId, string token);
    Task InvalidateAllUserTokensAsync(int userId);
    Task<PasswordResetToken> GetValidTokenAsync(int userId, string token);
}