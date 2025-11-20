using MediatR;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Queries;

public class GetAllUsersQuery : IRequest<List<User>>
{
}