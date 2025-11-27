using FluentValidation;
using ProductManagement.Application.Common.Interfaces;

namespace ProductManagement.Application.Validators.Common;

public class IdCommandValidator<T> : AbstractValidator<T> where T : IIdCommand
{
    public IdCommandValidator()
    {
        RuleFor(p => p.Id)
                .NotEmpty().WithMessage("ID обязателен.")
                .GreaterThan(0).WithMessage("ID должен быть положительным числом.");
    }
}