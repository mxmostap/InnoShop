using ProductManagement.Application.Commands;
using ProductManagement.Application.Validators.Common;

namespace ProductManagement.Application.Validators;

public class DeleteProductsByUserIdCommandValidator : IdCommandValidator<DeleteProductsByUserIdCommand>
{
    public DeleteProductsByUserIdCommandValidator()
    {
        
    }
}