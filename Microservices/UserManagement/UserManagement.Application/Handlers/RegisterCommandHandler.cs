using MediatR;
using UserManagement.Application.Commands;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Application.Exceptions;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Handlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    public RegisterCommandHandler(
        IUnitOfWork unitOfWork, 
        ITokenService tokenService, 
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    public async Task<int> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Users.GetUserByUsernameAsync(request.UserName) != null)
        {
            throw new UnauthorizedException("Данный логин уже существует.");
        }

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            EmailConfirmed = false,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsActive = true,
            Role = UserRole.User,
            Profile = new Profile
            {
                FirstName = request.FirstName,
                LastName = request.LastName
            }
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var confirmationToken = await _tokenService.GenerateAndSaveTokenAsync(
            user.Id,
            TokenAssignment.EmailConfirmation,
            TimeSpan.FromHours(24));

        await _emailService.SendEmailConfirmationAsync(user, confirmationToken);

        return user.Id;
    }
}