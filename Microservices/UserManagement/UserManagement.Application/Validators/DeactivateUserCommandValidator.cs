using UserManagement.Application.Commands;
using UserManagement.Application.Validators.Common;

namespace UserManagement.Application.Validators;

public class DeactivateUserCommandValidator : IdCommandValidator<DeactivateUserCommand>
{
    public DeactivateUserCommandValidator()
    {
        
    }
}