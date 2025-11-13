using UserManagement.Domain.Enums;

namespace UserManagement.Application.DTOs;

public class AuthResponse
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string JwtToken { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}