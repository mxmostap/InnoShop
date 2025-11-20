using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Infrastructure.Persistance;

namespace UserManagement.Infrastructure.Seeders;

public class DataSeeder
{
    private readonly IConfiguration _configuration;
    private readonly EFDBContext _context;

    public DataSeeder(IConfiguration configuration, EFDBContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public async Task SeedAdminInfoAsync()
    {
        var section = _configuration.GetSection("SeedData:Admin");
        if (!section.Exists())
            throw new InvalidOperationException("Раздел SeedData:Admin не найден в конфигурации.");
        var userName = section["UserName"];
        var email = section["Email"];
        var password = section["Password"];
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            throw new InvalidOperationException("Заполнены не все обязательные поля в конфигурации.");

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == userName);
        if (existingUser != null)
            return;
        
        var adminUser = new User
        {
            UserName = userName,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = UserRole.Admin,
            IsActive = true
        };

        _context.Users.Add(adminUser);
        await _context.SaveChangesAsync();
    }
}