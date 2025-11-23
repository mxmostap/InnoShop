using MediatR;

namespace ProductManagement.Application.Commands;

public class DeleteProductCommand : IRequest<Unit>
{
    public int ProductId { get; set; }
}