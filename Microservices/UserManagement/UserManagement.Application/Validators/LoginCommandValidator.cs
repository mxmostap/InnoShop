using FluentValidation;
using UserManagement.Application.Commands;

namespace UserManagement.Application.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(u => u.UserName)
            .NotEmpty()
            .WithMessage("Имя пользователя должно быть заполнено.");

        RuleFor(u => u.Password)
            .NotEmpty()
            .WithMessage("Пароль должен быть заполнен.");
    }
}