using System.Text.Json.Serialization;
using UserManagement.Domain.Enums;

namespace UserManagement.Domain.Entities;

public class Token
{
    public int Id { get; set; }
    public TokenAssignment Assignment { get; set; }
    public int UserId { get; set; }    
    public string TokenHash { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsUsed { get; set; }
    [JsonIgnore]
    public User User { get; set; }
    
    public bool IsValid => !IsUsed && ExpiresAt > DateTime.UtcNow;
}