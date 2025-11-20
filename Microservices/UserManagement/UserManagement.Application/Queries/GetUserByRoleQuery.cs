using MediatR;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Queries;

public class GetUsersByRoleQuery : IRequest<IEnumerable<User>>
{
    public string Role { get; }

    public GetUsersByRoleQuery(string role)
    {
        Role = role;
    }
}
