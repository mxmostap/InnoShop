using MediatR;
using ProductManagement.Microservice.Domain.Entities;

namespace ProductManagement.Application.Queries;

public class GetProductsByUserIdQuery : IRequest<IEnumerable<Product>>
{
    public int UserId { get; }

    public GetProductsByUserIdQuery(int userId)
    {
        UserId = userId;
    }
}