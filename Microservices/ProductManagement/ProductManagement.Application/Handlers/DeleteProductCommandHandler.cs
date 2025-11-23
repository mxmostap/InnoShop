using MediatR;
using ProductManagement.Application.Commands;
using ProductManagement.Application.Exceptions;
using ProductManagement.Application.Services;
using ProductManagement.Microservice.Domain.Repositories;

namespace ProductManagement.Application.Handlers;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        if (!int.TryParse(_currentUserService.UserId, out int userId))
            throw new UnauthorizedException("Пользователь не прошел аутентификацию.");
        
        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, userId);
        if (product == null)
            throw new NotFoundException($"Продукт с ID \"{request.ProductId}\" не найден " +
                                        $"или у вас нет права на его удаление.");
        
        _unitOfWork.Products.DeleteProduct(product);
        await _unitOfWork.SaveChangesAsync();
        
        return Unit.Value;
    }
}