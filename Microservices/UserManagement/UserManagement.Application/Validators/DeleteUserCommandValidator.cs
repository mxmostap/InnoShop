using UserManagement.Application.Commands;
using UserManagement.Application.Validators.Common;

namespace UserManagement.Application.Validators;

public class DeleteUserCommandValidator : IdCommandValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        
    }
}