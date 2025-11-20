using MediatR;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Queries;

public class GetUserByIdQuery : IRequest<User>
{
    public int Id { get; set; }
}