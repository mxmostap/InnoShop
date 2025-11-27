using ProductManagement.Application.Commands;
using ProductManagement.Application.Validators.Common;

namespace ProductManagement.Application.Validators;

public class DeleteProductCommandValidator : IdCommandValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        
    }
}