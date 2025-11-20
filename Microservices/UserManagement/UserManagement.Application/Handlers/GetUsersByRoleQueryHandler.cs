using MediatR;
using UserManagement.Application.Queries;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Handlers;

public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, IEnumerable<User>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUsersByRoleQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<User>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Users.GetUsersByRoleAsync(request.Role);
    }
}