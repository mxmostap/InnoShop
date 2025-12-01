using FluentValidation;
using UserManagement.Application.Commands;
using UserManagement.Application.Validators.Common;

namespace UserManagement.Application.Validators;

public class UpdateUserCommandValidator : UserBaseValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        
    }
}