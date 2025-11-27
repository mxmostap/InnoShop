using FluentValidation;
using ProductManagement.Application.Commands;
using ProductManagement.Application.Validators.Common;
using ProductManagement.Application.Validators.Extensions;

namespace ProductManagement.Application.Validators;

public class UpdateProductCommandValidator : ProductBaseValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("ID обязателен.")
            .GreaterThan(0).WithMessage("ID должен быть положительным числом.");
    }
}