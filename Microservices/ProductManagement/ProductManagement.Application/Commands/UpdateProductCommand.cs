using MediatR;
using ProductManagement.Microservice.Domain.Entities;

namespace ProductManagement.Application.Commands;

public class UpdateProductCommand : IRequest<Product>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool Availability { get; set; } = true;
}