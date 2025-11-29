using Microsoft.EntityFrameworkCore;
using ProductManagement.Microservice.Domain.Entities;

namespace ProductManagement.Infrastructure.Persistence;

public class EFDBContext : DbContext
{
    public EFDBContext(DbContextOptions<EFDBContext> options) : base (options)
    {
        Database.Migrate();
    }
    
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(p => p.Description)
                .HasMaxLength(255);

            entity.Property(p => p.Price)
                .HasColumnType("decimal(10,2)");

            entity.Property(p => p.CreatedAt)
                .HasColumnType("date");
        });
    }
}