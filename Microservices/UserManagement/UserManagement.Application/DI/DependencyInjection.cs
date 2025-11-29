using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Application.Services;
using UserManagement.Domain.Repositories;
using UserManagement.Infrastructure.Persistance;
using UserManagement.Infrastructure.Repositories;

namespace UserManagement.Application.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<EFDBContext>(options =>
            options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}