using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Application.Interfaces;
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
        //var connectionString = configuration.GetConnectionString("DefaultConnection");
        var qwe = configuration["ConnectionStrings:DefaultConnection"];
        Console.WriteLine(qwe);
        services.AddDbContext<EFDBContext>(options =>
            options.UseSqlServer(qwe));
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}