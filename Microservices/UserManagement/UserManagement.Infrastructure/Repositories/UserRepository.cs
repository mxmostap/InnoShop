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
            .SingleOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<IEnumerable<User>> GetUserByRoleAsync(string role)
    {
        return await _context.Users
            .Include(u => u.Profile)
            .Where(u => u.Role == UserRoleExtensions.FromString(role))
            .ToListAsync();
    }
}