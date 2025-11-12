using System.Text.Json.Serialization;

namespace UserManagement.Domain.Entities;

public class Profile
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int UserId { get; set; }
    
    [JsonIgnore]
    public User User { get; set; } = null!;
}