using System.Security.Cryptography;
using MediatR;
using UserManagement.Application.Commands;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Application.Exceptions;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Handlers;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly IEmailService _emailService;

    public ResetPasswordCommandHandler(
        IUnitOfWork unitOfWork, 
        IPasswordResetTokenService passwordResetTokenService,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _passwordResetTokenService = passwordResetTokenService;
        _emailService = emailService;
    }
    
    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetUserByEmailAsync(request.Email);
        if (user == null)
            throw new NotFoundException("Ошибка получения запроса.");

        var resetToken = await _passwordResetTokenService.GenerateAndSaveResetTokenAsync(user.Id, 
            TimeSpan.FromHours(24));
        
        await _emailService.SendPasswordResetEmailAsync(user, resetToken);
        
        return Unit.Value;
    }
}