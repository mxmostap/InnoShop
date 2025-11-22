using MediatR;
using ProductManagement.Application.Queries;
using ProductManagement.Microservice.Domain.Entities;
using ProductManagement.Microservice.Domain.Repositories;

namespace ProductManagement.Application.Handlers;

public class GetAllAvailableProductsQueryHandler : 
    IRequestHandler<GetAllAvailableProductsQuery, IEnumerable<Product>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllAvailableProductsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Product>> Handle(GetAllAvailableProductsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Products.GetAllAvailableAsync();
    }
}