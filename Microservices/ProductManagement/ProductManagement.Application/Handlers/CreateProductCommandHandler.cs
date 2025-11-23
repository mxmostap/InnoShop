using MediatR;
using ProductManagement.Application.Commands;
using ProductManagement.Application.Exceptions;
using ProductManagement.Application.Services;
using ProductManagement.Microservice.Domain.Entities;
using ProductManagement.Microservice.Domain.Repositories;

namespace ProductManagement.Application.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateProductCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Products.ExistProductByName(request.Name))
            throw new DataExistsException($"Продукт с навзанием {request.Name} уже существует.");
        
        if (!int.TryParse(_currentUserService.UserId, out int userId))
            throw new UnauthorizedException("Пользователь не прошел аутентификацию.");

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Availability = request.Availability,
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
            UserId = userId,
            IsDeleted = false
        };
        
        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return product;
    }
}