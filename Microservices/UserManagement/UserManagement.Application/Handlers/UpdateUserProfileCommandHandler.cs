using MediatR;
using UserManagement.Application.Commands;
using UserManagement.Application.Exceptions;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Handlers;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Profile>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserProfileCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async  Task<Profile> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _unitOfWork.Profiles.GetByIdAsync(request.UserId);
        if (profile == null)
            throw new NotFoundException($"Пользователь с ID \"{request.UserId}\" не найден.");

        profile.FirstName = request.FirstName;
        profile.LastName = request.LastName;

        await _unitOfWork.SaveChangesAsync();

        return profile;
    }
}