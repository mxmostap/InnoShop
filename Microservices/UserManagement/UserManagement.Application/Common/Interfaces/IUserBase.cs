using UserManagement.Domain.Enums;

namespace UserManagement.Application.Common.Interfaces;

public interface IUserBase
{
    public string UserName { get; set; }
    public string Email { get; set; }
}