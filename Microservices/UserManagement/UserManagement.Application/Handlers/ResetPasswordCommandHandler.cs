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
    private readonly IEmailService _emailService;

    public ResetPasswordCommandHandler(IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }
    
    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetUsersByEmailAsync(request.Email);
        if (user == null)
            throw new NotFoundException("Ошибка получения запроса.");

        var resetToken = GenerateResetToken();

        await _emailService.SendPasswordResetEmailAsync(user, resetToken);
        
        return Unit.Value;
    }
    
    private string GenerateResetToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
            
        return Convert.ToBase64String(randomBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
    }
}