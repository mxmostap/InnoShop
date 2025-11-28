using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Repositories;
using UserManagement.Infrastructure.Persistance;

namespace UserManagement.Infrastructure.Repositories;

public class ProfileRepository: GenericRepository<Profile, int>, IProfileRepository
{
    public ProfileRepository(EFDBContext context) : base(context) { }

    public async Task<Profile> GetByUserIdAsync(int userId)
    {
        return await _context.Profiles
            .Include(p => p.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == userId);
    }
}