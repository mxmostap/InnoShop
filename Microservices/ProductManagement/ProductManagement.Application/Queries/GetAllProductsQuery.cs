using MediatR;
using ProductManagement.Microservice.Domain.Entities;

namespace ProductManagement.Application.Queries;

public class GetAllProductsQuery : IRequest<IEnumerable<Product>>
{
}