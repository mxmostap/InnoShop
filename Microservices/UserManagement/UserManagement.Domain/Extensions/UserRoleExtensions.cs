using UserManagement.Domain.Enums;

namespace UserManagement.Domain.Extensions;

public static class UserRoleExtensions
{
    public static string ToString(this UserRole role)
    {
        return role switch
        {
            UserRole.User => "User",
            UserRole.Admin => "Admin",
            _ => "User"
        };
    }

    public static UserRole FromString(this string role)
    {
        return role?.ToLower() switch
        {
            "admin" => UserRole.Admin,
            "user" => UserRole.User,
            _ => UserRole.User
        };
    }

    public static bool IsAdmin(this UserRole role) => role == UserRole.Admin;
}