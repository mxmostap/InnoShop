using MediatR;
using UserManagement.Application.Commands;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Application.DTOs;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Handlers;

public class ResetPasswordConfirmCommandHandler : 
    IRequestHandler<ResetPasswordConfirmCommand, ResetPasswordConfirmCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public ResetPasswordConfirmCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }
    
    public async Task<ResetPasswordConfirmCommandResult> Handle(
        ResetPasswordConfirmCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetUserByEmailAsync(request.Email);
        if(user == null)
            return ResetPasswordConfirmCommandResult.Failed("Неверный токен или Email.");

        var isValidToken = await _tokenService.ValidateTokenAsync(
            user.Id, request.Token, TokenAssignment.PasswordReset);
        if (!isValidToken)
            return ResetPasswordConfirmCommandResult.Failed("Неверный или просроченный токен.");
        
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _unitOfWork.SaveChangesAsync();

        await _tokenService.InvalidateTokenAsync(user.Id, request.Token, TokenAssignment.PasswordReset);
        
        return ResetPasswordConfirmCommandResult.Successful();
    }
}