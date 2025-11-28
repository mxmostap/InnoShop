using UserManagement.Domain.Entities;

namespace UserManagement.Domain.Repositories;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken> GetValidTokenAsync(int userId, string tokenHash);
    Task<List<PasswordResetToken>> GetUserTokensAsync(int userId);
    Task AddAsync(PasswordResetToken token);
    Task UpdateAsync(PasswordResetToken token);
    Task InvalidateUserTokensAsync(int userId);
    Task DeleteExpiredTokensAsync();
}