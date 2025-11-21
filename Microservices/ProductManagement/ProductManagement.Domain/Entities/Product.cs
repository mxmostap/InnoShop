using System.Text.Json.Serialization;

namespace ProductManagement.Microservice.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Price { get; set; }
    public bool Availability { get; set; } = true;
    public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public int UserId { get; set; }
    public bool IsDeleted { get; set; } = false;
}