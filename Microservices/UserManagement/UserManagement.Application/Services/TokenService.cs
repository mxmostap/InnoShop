using System.Security.Cryptography;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Services;

public class TokenService : ITokenService
{
    private readonly ITokenRepository _tokenRepository;

    public TokenService(ITokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }
    
    public async Task<string> GenerateAndSaveTokenAsync(int userId, TokenAssignment assignment, TimeSpan expiration)
    {
        var token = GenerateSecureToken();
        var tokenHash = HashToken(token);
        var expirationTime = DateTime.UtcNow.Add(expiration);

        await _tokenRepository.InvalidateTokensAsync(userId, assignment);

        var tokenEntity  = new Token
        {
            UserId = userId,
            Assignment = assignment,
            TokenHash = tokenHash,
            ExpiresAt = expirationTime,
            CreatedAt = DateTime.UtcNow,
            IsUsed = false
        };

        await _tokenRepository.AddTokenAsync(tokenEntity);

        return token;
    }

    public async Task<bool> ValidateTokenAsync(int userId, string token, TokenAssignment assignment)
    {
        var tokenHash = HashToken(token);
        var tokenEntity  = await _tokenRepository.GetValidTokenAsync(userId, tokenHash, assignment);
        return tokenEntity  != null && tokenEntity.IsValid; 
    }

    public async Task InvalidateTokenAsync(int userId, string token, TokenAssignment assignment)
    {
        var tokenHash = HashToken(token);
        var tokenEntity = await _tokenRepository.GetValidTokenAsync(userId, tokenHash, assignment);
            
        if (tokenEntity != null)
        {
            tokenEntity.IsUsed = true;
            await _tokenRepository.UpdateAsync(tokenEntity);
        }
    }

    public async Task InvalidateAllUserTokensAsync(int userId, TokenAssignment assignment)
    {
        await _tokenRepository.InvalidateTokensAsync(userId, assignment);
    }

    public async Task<Token> GetValidTokenAsync(int userId, string token, TokenAssignment assignment)
    {
        var tokenHash = HashToken(token);
        return await _tokenRepository.GetValidTokenAsync(userId, tokenHash, assignment);
    }
    
    private string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
            
        return Convert.ToBase64String(randomBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
    }
    
    private string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashedBytes);
    }
}