using MediatR;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.Commands;

public class ConfirmEmailCommand : IRequest<ConfirmEmailCommandResult>
{
    public string Email { get; set; }
    public string Token { get; set; }
}