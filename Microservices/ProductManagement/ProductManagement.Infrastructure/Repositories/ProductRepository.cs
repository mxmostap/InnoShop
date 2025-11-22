using Microsoft.EntityFrameworkCore;
using ProductManagement.Infrastructure.Persistence;
using ProductManagement.Microservice.Domain.Entities;
using ProductManagement.Microservice.Domain.Repositories;

namespace ProductManagement.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product, int>, IProductRepository
{
    public ProductRepository(EFDBContext context) : base(context) { }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await WhereNotDeleted(_context.Products)
            .ToListAsync();
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        return await WhereNotDeleted(_context.Products)
            .SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetByUserIdAsync(int userId)
    {
        return await WhereNotDeleted(_context.Products)
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllAvailableAsync()
    {
        return await WhereNotDeleted(_context.Products)
            .Where(p => p.Availability == true)
            .ToListAsync();
    }

    private static IQueryable<Product> WhereNotDeleted(IQueryable<Product> query)
    {
        return query.Where(p => !p.IsDeleted);
    }
}