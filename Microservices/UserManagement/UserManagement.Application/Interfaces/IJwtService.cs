using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    int? ValidateToken(string token);
}