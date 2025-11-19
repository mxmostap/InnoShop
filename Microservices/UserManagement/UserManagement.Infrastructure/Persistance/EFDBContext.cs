using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Infrastructure.Persistance;

public class EFDBContext: DbContext
{
    public EFDBContext(DbContextOptions<EFDBContext> options) : base (options)
    {
        Database.Migrate();
    }

    protected EFDBContext()
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Profile> Profiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.HasIndex(u => u.UserName)
                .IsUnique();

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<Profile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.HasIndex(p => p.UserId)
                .IsUnique();
        });
        
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                UserName = "admin",
                Email = "mxmostapwork@gmail.com",
                PasswordHash = "$2a$11$rLZeB6R2kS5Wq2qKkE5M5eMvJQY9W5ZQY5X5X5X5X5X5X5X5X5X5",
                Role = UserRole.Admin,
                IsActive = true
            }
        );
    }
}