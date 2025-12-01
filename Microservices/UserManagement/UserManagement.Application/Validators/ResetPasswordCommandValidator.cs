using FluentValidation;
using UserManagement.Application.Commands;

namespace UserManagement.Application.Validators;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Введено недопустимое значение Email.");
    }
}