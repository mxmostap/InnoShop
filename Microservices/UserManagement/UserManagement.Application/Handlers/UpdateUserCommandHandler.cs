using MediatR;
using UserManagement.Application.Commands;
using UserManagement.Application.Exceptions;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Handlers;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
            throw new NotFoundException($"Пользователь с ID \"{request.UserId}\" не найден.");

        user.UserName = request.UserName;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        user.Email = request.Email;

        await _unitOfWork.SaveChangesAsync();
        
        return user;
    }
}