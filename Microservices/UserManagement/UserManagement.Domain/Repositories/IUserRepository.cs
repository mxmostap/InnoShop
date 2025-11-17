using UserManagement.Domain.Entities;

namespace UserManagement.Domain.Repositories;

public interface IUserRepository: IGenericRepository<User, int>
{
    Task<User> GetUserByUsernameAsync(string userName);
    Task<User> GetUserByEmailAsync(string userEmail);
    Task<IEnumerable<User>> GetUserByRoleAsync(string role);
    
}