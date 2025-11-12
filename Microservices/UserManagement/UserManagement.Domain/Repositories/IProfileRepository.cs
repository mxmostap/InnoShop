using UserManagement.Domain.Entities;

namespace UserManagement.Domain.Repositories;

public interface IProfileRepository
{
    Task<Profile> GetByUserIdAsync(int userId);
}