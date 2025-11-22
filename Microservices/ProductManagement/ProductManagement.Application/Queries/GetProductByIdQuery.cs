using MediatR;
using ProductManagement.Microservice.Domain.Entities;

namespace ProductManagement.Application.Queries;

public class GetProductByIdQuery : IRequest<Product>
{
    public int Id { get; }

    public GetProductByIdQuery(int id)
    {
        Id = id;
    }
}