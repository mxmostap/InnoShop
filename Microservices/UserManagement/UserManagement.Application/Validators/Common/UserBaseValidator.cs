using FluentValidation;
using UserManagement.Application.Common.Interfaces;

namespace UserManagement.Application.Validators.Common;

public class UserBaseValidator<T> : AbstractValidator<T> where T : IUserBase
{
    public UserBaseValidator()
    {
        RuleFor(u => u.UserName)
            .NotEmpty()
            .WithMessage("Username не может быть пустым.");

        RuleFor(u => u.Email)
            .NotEmpty()
            .WithMessage("Email не может быть пустым.");
        
        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email не соответсвует формату(должен присутствовать символ @).");
    }
}