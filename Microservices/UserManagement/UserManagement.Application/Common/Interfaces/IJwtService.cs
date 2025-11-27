using UserManagement.Domain.Entities;

namespace UserManagement.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}