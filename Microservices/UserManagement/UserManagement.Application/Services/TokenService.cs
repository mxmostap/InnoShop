using Microsoft.Extensions.Configuration;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        
        return "";
    }
}