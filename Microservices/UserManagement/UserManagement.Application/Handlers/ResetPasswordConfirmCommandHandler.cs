using MediatR;
using UserManagement.Application.Commands;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Application.DTOs;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Handlers;

public class ResetPasswordConfirmCommandHandler : 
    IRequestHandler<ResetPasswordConfirmCommand, ResetPasswordConfirmCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordResetTokenService _passwordResetTokenService;

    public ResetPasswordConfirmCommandHandler(IUnitOfWork unitOfWork, IPasswordResetTokenService passwordResetTokenService)
    {
        _unitOfWork = unitOfWork;
        _passwordResetTokenService = passwordResetTokenService;
    }
    
    public async Task<ResetPasswordConfirmCommandResult> Handle(
        ResetPasswordConfirmCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetUserByEmailAsync(request.Email);
        if(user == null)
            return ResetPasswordConfirmCommandResult.Failed("Неверный токен или Email.");

        var isValidToken = await _passwordResetTokenService.ValidateResetTokenAsync(user.Id, request.Token);
        if (!isValidToken)
            return ResetPasswordConfirmCommandResult.Failed("Неверный или просроченный токен.");

        var resetToken = await _passwordResetTokenService.GetValidTokenAsync(user.Id, request.Token);
        if(resetToken == null)
            return ResetPasswordConfirmCommandResult.Failed("Токен не найден.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        await _passwordResetTokenService.InvalidateResetTokenAsync(user.Id, request.Token);
        
        return ResetPasswordConfirmCommandResult.Successful();
    }
}