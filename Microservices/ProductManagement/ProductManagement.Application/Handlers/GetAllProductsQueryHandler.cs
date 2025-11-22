using MediatR;
using ProductManagement.Application.Queries;
using ProductManagement.Microservice.Domain.Entities;
using ProductManagement.Microservice.Domain.Repositories;

namespace ProductManagement.Application.Handlers;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<Product>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllProductsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Products.GetAllAsync();
    }
}