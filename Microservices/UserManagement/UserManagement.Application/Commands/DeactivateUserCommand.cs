using MediatR;

namespace UserManagement.Application.Commands;

public class DeactivateUserCommand : IRequest<Unit>
{
    public int UserId { get; set; }
}