using System.Security.Cryptography;
using MediatR;
using UserManagement.Application.Commands;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Application.Exceptions;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Handlers;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    public ResetPasswordCommandHandler(
        IUnitOfWork unitOfWork, 
        ITokenService tokenService,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _emailService = emailService;
    }
    
    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetUserByEmailAsync(request.Email);
        if (user == null)
            throw new NotFoundException("Ошибка получения запроса.");

        var resetToken = await _tokenService.GenerateAndSaveTokenAsync(
            user.Id, 
            TokenAssignment.PasswordReset,
            TimeSpan.FromHours(24));
        
        await _emailService.SendPasswordResetEmailAsync(user, resetToken);
        
        return Unit.Value;
    }
}