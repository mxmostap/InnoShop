using FluentValidation;
using ProductManagement.Application.Commands;

namespace ProductManagement.Application.Validators;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
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

        RuleFor(p => p.Availability)
            .NotEmpty()
            .WithMessage("Доступность должна быть указана.");
    }
}