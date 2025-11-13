using UserManagement.Domain.Entities;

namespace UserManagement.Application.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}