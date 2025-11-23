using MediatR;

namespace ProductManagement.Application.Commands;

public class SoftDeleteProductsCommand : IRequest<Unit>
{
    public int UserId { get; set; }
}