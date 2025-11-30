using MediatR;
using UserManagement.Application.Commands;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Application.DTOs;
using UserManagement.Application.Exceptions;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {             
        var user = await _unitOfWork.Users.GetUserByUsernameAsync(request.UserName);
        
        if (user == null || !user.IsActive)
            throw new UnauthorizedException("Неверные учетные данные.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Неверный пароль.");

        var token = _jwtService.GenerateToken(user);

        return new AuthResponse
        {
            JwtToken = token,
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.Profile?.FirstName ?? string.Empty,
            LastName = user.Profile?.LastName ?? string.Empty,
            Role = user.Role
        };
    }
}