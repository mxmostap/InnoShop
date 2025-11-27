using MediatR;
using ProductManagement.Application.Commands;
using ProductManagement.Microservice.Domain.Repositories;

namespace ProductManagement.Application.Handlers;

public class DeleteProductsByUserIdCommandHandler : IRequestHandler<DeleteProductsByUserIdCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductsByUserIdCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteProductsByUserIdCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Products.DeleteProductsByUserIdAsync(request.Id);
        await _unitOfWork.SaveChangesAsync();
        
        return Unit.Value;
    }
}