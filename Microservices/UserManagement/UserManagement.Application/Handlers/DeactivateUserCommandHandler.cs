using System.Globalization;
using MediatR;
using UserManagement.Application.Commands;
using UserManagement.Application.Exceptions;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Handlers;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
            throw new NotFoundException($"Пользователь с ID \"{request.UserId}\" не найден.");

        user.IsActive = !user.IsActive;
        await _unitOfWork.SaveChangesAsync();
        
        return Unit.Value;
    }
}