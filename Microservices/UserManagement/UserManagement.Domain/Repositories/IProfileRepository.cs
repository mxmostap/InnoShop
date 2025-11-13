using UserManagement.Domain.Entities;

namespace UserManagement.Domain.Repositories;

public interface IProfileRepository : IGenericRepository<Profile, int>
{
    Task<Profile> GetByUserIdAsync(int userId);
}