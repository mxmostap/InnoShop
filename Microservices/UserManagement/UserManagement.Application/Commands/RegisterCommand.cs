using MediatR;
using UserManagement.Application.Common.Interfaces;

namespace UserManagement.Application.Commands;

public class RegisterCommand : IRequest<int>, IUserBase
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}