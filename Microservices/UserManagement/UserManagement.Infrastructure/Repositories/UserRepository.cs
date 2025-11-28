using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Extensions;
using UserManagement.Domain.Repositories;
using UserManagement.Infrastructure.Persistance;

namespace UserManagement.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User, int>, IUserRepository
{
    public UserRepository(EFDBContext context) : base(context) { }

    public async Task<User> GetUserByUsernameAsync(string userName)
    {
        return await _context.Users
            .Include(u => u.Profile)
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.UserName == userName);
    }
    
    public async Task<User> GetUserByEmailAsync(string userEmail)
    {
        return await _context.Users
            .Include(u => u.Profile)            
            .SingleOrDefaultAsync(u => u.Email == userEmail);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
    {
        return await _context.Users
            .Include(u => u.Profile)
            .AsNoTracking()
            .Where(u => u.Role == UserRoleExtensions.FromString(role))
            .ToListAsync();
    }

    public async Task UpdateEmailConfirmedAsync(int userId, bool confirmed)
    {
        var user = new User { Id = userId, EmailConfirmed = confirmed };
        _context.Users.Attach(user);
        _context.Entry(user).Property(x => x.EmailConfirmed).IsModified = true;
    }
}