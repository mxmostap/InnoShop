using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;
using UserManagement.Infrastructure.Persistance;

namespace UserManagement.Infrastructure.Repositories;

public class TokenRepository :ITokenRepository
{
    private readonly EFDBContext _context;

    public TokenRepository(EFDBContext context)
    {
        _context = context;
    }
    
    public async Task<Token> GetValidTokenAsync(int userId, string tokenHash, TokenAssignment assignment)
    {
        return await _context.Tokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t =>
                t.UserId == userId &&
                t.Assignment == assignment &&
                t.TokenHash == tokenHash &&
                !t.IsUsed && 
                t.ExpiresAt > DateTime.UtcNow);
    }
    
    public async Task AddTokenAsync(Token token)
    {
        await _context.Tokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Token token)
    {
        var existingEntity = _context.ChangeTracker.Entries<Token>()
            .FirstOrDefault(e => e.Entity.Id == token.Id);
    
        if (existingEntity != null)
        {
            _context.Entry(existingEntity.Entity).CurrentValues.SetValues(token);
        }
        else
        {
            _context.Tokens.Update(token);
            await _context.SaveChangesAsync();
        }
        _context.Tokens.Attach(token);
        _context.Entry(token).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
    
    public async Task InvalidateTokensAsync(int userId, TokenAssignment assignment)
    {
        var tokens = await _context.Tokens
            .Where(t => t.UserId == userId && 
                        t.Assignment == assignment &&
                        !t.IsUsed)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsUsed = true;
        }

        await _context.SaveChangesAsync();
    }
}