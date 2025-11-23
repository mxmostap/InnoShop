using MediatR;

namespace ProductManagement.Application.Commands;

public class DeleteProductsByUserIdCommand : IRequest<Unit>
{
    public int UserId { get; set; }
}