using MediatR;

namespace UserManagement.Application.Commands;

public class DeleteUserCommand : IRequest<Unit>
{
    public int UserId { get; set; }
}