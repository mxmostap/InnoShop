using ProductManagement.Microservice.Domain.Repositories;

namespace ProductManagement.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly EFDBContext _context;
    public IProductRepository Products { get; }

    public UnitOfWork(
        EFDBContext context,
        IProductRepository products)
    {
        _context = context;
        Products = products;
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}