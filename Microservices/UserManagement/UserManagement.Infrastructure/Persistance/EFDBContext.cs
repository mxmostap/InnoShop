using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Extensions;

namespace UserManagement.Infrastructure.Persistance;

public class EFDBContext: DbContext
{
    public EFDBContext(DbContextOptions<EFDBContext> options) : base (options)
    {
        Database.Migrate();
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Token> Tokens { get; set; }

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

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.TokenHash)
                .IsRequired()
                .HasMaxLength(255);
        });
        
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Email = "mxmostapwork@gmail.com",
                EmailConfirmed = true,
                IsActive = true,
                PasswordHash = "$2a$11$S9/g78Q0Q25AbhHyG0btFOtbhW7BQHqN9NKLoKxCTKDawXrF9g8nG",
                Role = UserRole.Admin,
                UserName = "admin"
            });
    }
}