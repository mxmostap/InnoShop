using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Repositories;
using UserManagement.Infrastructure.Persistance;

namespace UserManagement.Infrastructure.Repositories;

public class PasswordResetTokenRepository :IPasswordResetTokenRepository
{
    private readonly EFDBContext _context;

    public PasswordResetTokenRepository(EFDBContext context)
    {
        _context = context;
    }
    
    public async Task<PasswordResetToken> GetValidTokenAsync(int userId, string tokenHash)
    {
        return await _context.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t =>
                t.UserId == userId &&
                t.TokenHash == tokenHash &&
                t.IsValid);
    }

    public async Task AddAsync(PasswordResetToken token)
    {
        await _context.PasswordResetTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PasswordResetToken token)
    {
        _context.PasswordResetTokens.Update(token);
        await _context.SaveChangesAsync();
    }

    public async Task InvalidateUserTokensAsync(int userId)
    {
        var tokens = await _context.PasswordResetTokens
            .Where(t => t.UserId == userId && !t.IsUsed)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsUsed = true;
        }

        await _context.SaveChangesAsync();
    }
}