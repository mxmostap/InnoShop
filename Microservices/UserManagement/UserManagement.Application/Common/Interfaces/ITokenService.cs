using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Application.Common.Interfaces;

public interface ITokenService
{
    Task<string> GenerateAndSaveTokenAsync(int userId, TokenAssignment assignment, TimeSpan expiration);
    Task<bool> ValidateTokenAsync(int userId, string token, TokenAssignment assignment);
    Task InvalidateTokenAsync(int userId, string token, TokenAssignment assignment);
    Task InvalidateAllUserTokensAsync(int userId, TokenAssignment assignment);
    Task<Token> GetValidTokenAsync(int userId, string token, TokenAssignment assignment);
}