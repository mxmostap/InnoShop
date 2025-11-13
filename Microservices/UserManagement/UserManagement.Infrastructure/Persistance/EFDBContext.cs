using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Persistance;

public class EFDBContext: DbContext
{
    public EFDBContext(DbContextOptions options) : base (options)
    {
        
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Profile> Profiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EFDBContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}