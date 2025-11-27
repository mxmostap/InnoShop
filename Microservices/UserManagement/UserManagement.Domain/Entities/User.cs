using UserManagement.Domain.Enums;

namespace UserManagement.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; } = false;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public UserRole Role { get; set; }

    public Profile? Profile { get; set; }
}