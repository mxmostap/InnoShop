using MediatR;
using ProductManagement.Application.Commands;
using ProductManagement.Application.Exceptions;
using ProductManagement.Application.Services;
using ProductManagement.Microservice.Domain.Entities;
using ProductManagement.Microservice.Domain.Repositories;

namespace ProductManagement.Application.Handlers;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Product>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateProductCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Product> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        if (!int.TryParse(_currentUserService.UserId, out int userId))
            throw new UnauthorizedException("Пользователь не прошел аутентификацию.");
        
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, userId);
        if (product == null)
            throw new NotFoundException($"Продукт с ID \"{request.Id}\" не найден " +
                                        $"или у вас нет права на его удаление.");

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Availability = request.Availability;

        _unitOfWork.Products.UpdateProduct(product);

        await _unitOfWork.SaveChangesAsync();
        return product;
    }
}