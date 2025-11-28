using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Infrastructure.Persistance;

namespace UserManagement.Infrastructure.Extensions;

public static class DatabaseMigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this IServiceProvider services)
    {
        try
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EFDBContext>();
            var database = dbContext.Database;
        
            if (!await database.CanConnectAsync())
            {
                await database.EnsureCreatedAsync();
            }
        
            var pendingMigrations = await database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                await database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Ошибка проведения миграций БД", ex);
        }
    }
}