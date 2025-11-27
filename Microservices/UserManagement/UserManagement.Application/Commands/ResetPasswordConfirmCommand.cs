using MediatR;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.Commands;

public class ResetPasswordConfirmCommand : IRequest<ResetPasswordConfirmCommandResult>
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}