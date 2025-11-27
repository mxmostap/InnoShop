using ProductManagement.Application.Commands;
using ProductManagement.Application.Validators.Common;

namespace ProductManagement.Application.Validators;

public class SoftDeleteProductsCommandValidator : IdCommandValidator<SoftDeleteProductsCommand>
{
    public SoftDeleteProductsCommandValidator()
    {
        
    }
}