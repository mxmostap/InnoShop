using MediatR;
using ProductManagement.Application.Queries;
using ProductManagement.Microservice.Domain.Entities;
using ProductManagement.Microservice.Domain.Repositories;

namespace ProductManagement.Application.Handlers;

public class GetProductsByUserIdQueryHandler : 
    IRequestHandler<GetProductsByUserIdQuery, IEnumerable<Product>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductsByUserIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Product>> Handle(GetProductsByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Products.GetByUserIdAsync(request.UserId);
    }
}