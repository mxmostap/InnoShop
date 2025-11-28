using System.Security.Cryptography;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Services;

public class PasswordResetTokenService : IPasswordResetTokenService
{
    private readonly IPasswordResetTokenRepository _tokenRepository;

    public PasswordResetTokenService(IPasswordResetTokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }
    
    public async Task<string> GenerateAndSaveResetTokenAsync(int userId, TimeSpan expiration)
    {
        var token = GenerateSecureToken();
        var tokenHash = HashToken(token);
        var expirationTime = DateTime.UtcNow.Add(expiration);

        await _tokenRepository.InvalidateUserTokensAsync(userId);

        var resetToken = new PasswordResetToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = expirationTime,
            CreatedAt = DateTime.UtcNow,
            IsUsed = false
        };

        await _tokenRepository.AddAsync(resetToken);

        return token;
    }

    public async Task<bool> ValidateResetTokenAsync(int userId, string token)
    {
        var tokenHash = HashToken(token);
        var resetToken = await _tokenRepository.GetValidTokenAsync(userId, tokenHash);
        return resetToken != null && resetToken.IsValid; 
    }

    public async Task InvalidateResetTokenAsync(int userId, string token)
    {
        var tokenHash = HashToken(token);
        var resetToken = await _tokenRepository.GetValidTokenAsync(userId, tokenHash);
            
        if (resetToken != null)
        {
            resetToken.IsUsed = true;
            await _tokenRepository.UpdateAsync(resetToken);
        }
    }

    public async Task InvalidateAllUserTokensAsync(int userId)
    {
        await _tokenRepository.InvalidateUserTokensAsync(userId);
    }

    public async Task<PasswordResetToken> GetValidTokenAsync(int userId, string token)
    {
        var tokenHash = HashToken(token);
        return await _tokenRepository.GetValidTokenAsync(userId, tokenHash);
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