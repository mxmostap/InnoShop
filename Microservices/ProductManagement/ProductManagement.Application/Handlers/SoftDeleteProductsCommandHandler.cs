using MediatR;
using ProductManagement.Application.Commands;
using ProductManagement.Microservice.Domain.Repositories;

namespace ProductManagement.Application.Handlers;

public class SoftDeleteProductsCommandHandler : IRequestHandler<SoftDeleteProductsCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeleteProductsCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(SoftDeleteProductsCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Products.SoftDeleteProductsAsync(request.UserId);
        
        return Unit.Value;
    }
}