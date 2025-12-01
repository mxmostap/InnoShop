using FluentValidation;
using UserManagement.Application.Commands;

namespace UserManagement.Application.Validators;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(u => u.FirstName)
            .NotEmpty()
            .WithMessage("Имя пользователя не может быть пустым.");
        
        RuleFor(u => u.FirstName)
            .NotEmpty()
            .WithMessage("Фамилия пользователя не может быть пустой.");
    }
}