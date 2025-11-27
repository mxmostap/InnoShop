using MediatR;

namespace UserManagement.Application.Commands;

public class ResetPasswordCommand : IRequest<Unit>
{
    public string Email { get; set; } }