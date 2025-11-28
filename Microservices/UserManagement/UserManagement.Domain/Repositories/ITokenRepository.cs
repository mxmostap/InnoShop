using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Domain.Repositories;

public interface ITokenRepository
{
    Task<Token> GetValidTokenAsync(int userId, string tokenHash, TokenAssignment assignment);
    Task AddTokenAsync(Token token);
    Task InvalidateTokensAsync(int userId, TokenAssignment assignment);
    Task UpdateAsync(Token token);
}