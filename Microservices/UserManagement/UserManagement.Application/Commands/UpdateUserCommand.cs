using MediatR;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Commands;

public class UpdateUserCommand : IRequest<User>
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}