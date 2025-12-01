using FluentValidation;
using UserManagement.Application.Commands;

namespace UserManagement.Application.Validators;

public class ResetPasswordConfirmCommandValidator : AbstractValidator<ResetPasswordConfirmCommand>
{
    public ResetPasswordConfirmCommandValidator()
    {
        RuleFor(u => u.NewPassword)
            .NotEmpty()
            .WithMessage("Новый пароль не должен быть пустым.");

        RuleFor(u => u.ConfirmPassword)
            .NotEmpty()
            .Equal(u => u.NewPassword)
            .WithMessage("Новый пароль и его подтверждение не совпадают.");
    }
}