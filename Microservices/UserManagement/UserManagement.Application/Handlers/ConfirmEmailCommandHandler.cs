using MediatR;
using UserManagement.Application.Commands;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Application.DTOs;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Handlers;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, ConfirmEmailCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public ConfirmEmailCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }
    
    public async Task<ConfirmEmailCommandResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetUserByEmailAsync(request.Email);
        if (user == null)
            return ConfirmEmailCommandResult.Failed("Пользователь не найден.");
        
        if (user.EmailConfirmed)
            return ConfirmEmailCommandResult.Failed("Email уже подтвержден.");
        
        var isValidToken = await _tokenService.ValidateTokenAsync(
            user.Id, request.Token, TokenAssignment.EmailConfirmation);
        
        if (!isValidToken)
            return ConfirmEmailCommandResult.Failed("Неверный или просроченный токен");

        _unitOfWork.DetachEntity(user);
        await _unitOfWork.Users.UpdateEmailConfirmedAsync(user.Id, true);
        
        await _unitOfWork.SaveChangesAsync();

        await _tokenService.InvalidateTokenAsync(user.Id, request.Token, TokenAssignment.EmailConfirmation);
        
        return ConfirmEmailCommandResult.Successful();
    }
}