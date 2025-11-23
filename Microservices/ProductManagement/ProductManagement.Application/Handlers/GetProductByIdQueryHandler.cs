using MediatR;
using Microsoft.AspNetCore.Http;
using ProductManagement.Application.Queries;
using ProductManagement.Microservice.Domain.Entities;
using ProductManagement.Microservice.Domain.Repositories;

namespace ProductManagement.Application.Handlers;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        
        return await _unitOfWork.Products.GetByIdAsync(request.Id);
    }
}