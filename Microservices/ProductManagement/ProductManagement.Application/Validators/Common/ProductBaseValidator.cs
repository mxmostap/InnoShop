using FluentValidation;
using ProductManagement.Application.Common.Interfaces;
using ProductManagement.Application.Validators.Extensions;

namespace ProductManagement.Application.Validators.Common;

public class ProductBaseValidator<T> : AbstractValidator<T> where T : IProductBase
{
    public ProductBaseValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Название товара не может быть пустым.");

        RuleFor(p => p.Description)
            .MaximumLength(255)
            .WithMessage("Описание товара не может быть более 255 символов.");

        RuleFor(p => p.Price)
            .Must(p => p > 0)
            .WithMessage("Стоимость должна быть больше нуля.");

        RuleFor(p => p.Price)
            .DecimalPrecision(2)
            .WithMessage("Стоимость должна быть не более двух знаком после запятой.");

        RuleFor(p => p.Availability)
            .NotEmpty()
            .WithMessage("Доступность должна быть указана.");
    }
}