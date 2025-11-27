using MediatR;
using ProductManagement.Application.Common.Interfaces;
using ProductManagement.Microservice.Domain.Entities;

namespace ProductManagement.Application.Commands;

public class CreateProductCommand : IRequest<Product>, IProductBase
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool Availability { get; set; } = true;
}