using MediatR;
using UserManagement.Application.Common.Interfaces;

namespace UserManagement.Application.Commands;

public class DeactivateUserCommand : IRequest<Unit>, IIdCommand
{
    public int UserId { get; set; }
}