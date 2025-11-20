using MediatR;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Commands;

public class UpdateUserProfileCommand : IRequest<Profile>
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}