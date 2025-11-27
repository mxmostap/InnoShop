using MediatR;
using UserManagement.Application.Commands;
using UserManagement.Application.DTOs;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Handlers;

public class ResetPasswordConfirmCommandHandler : 
    IRequestHandler<ResetPasswordConfirmCommand, ResetPasswordConfirmCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordConfirmCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<ResetPasswordConfirmCommandResult> Handle(
        ResetPasswordConfirmCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetUsersByEmailAsync(request.Email);
        if(user == null)
            return ResetPasswordConfirmCommandResult.Failed("Неверный токен или Email.");
        
        var tokenHash = HashToken(request.Token);
        
        //var resetToken = await _unitOfWork.Pass


        return ResetPasswordConfirmCommandResult.Successful();
    }
    
    private string HashToken(string token)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashedBytes);
        
    }
}