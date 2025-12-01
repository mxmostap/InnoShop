using FluentValidation;
using UserManagement.Application.Common.Interfaces;

namespace UserManagement.Application.Validators.Common;

public class IdCommandValidator<T> : AbstractValidator<T> where T : IIdCommand
{
    public IdCommandValidator()
    {
        RuleFor(u => u.UserId)
            .NotEmpty().WithMessage("ID обязателен.")
            .GreaterThan(0).WithMessage("ID должен быть положительным числом.");
    }
}