using Microsoft.EntityFrameworkCore;
using ProductManagement.Infrastructure.Persistence;
using ProductManagement.Microservice.Domain.Entities;
using ProductManagement.Microservice.Domain.Repositories;

namespace ProductManagement.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly EFDBContext _context;

    public ProductRepository(EFDBContext context)
    {
        _context = context;
    }

    public async Task<Product> GetByIdAsync(int productId, int? userId = null)
    {
        var query = WhereNotDeleted(_context.Products)
            .Where(p => p.Id == productId);

        if (userId.HasValue)
        {
            query = query.Where(p => p.UserId == userId.Value);
        }

        return await query.FirstOrDefaultAsync();
    }
    
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await WhereNotDeleted(_context.Products)
            .ToListAsync();
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

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public void DeleteProduct(Product product)
    {
        _context.Products.Remove(product);
    }
    
    public async Task DeleteProductsByUserIdAsync(int userId)
    {
        var products = await _context.Products
            .Where(p => p.UserId == userId)
            .ToListAsync();
        if (products.Any())
            _context.RemoveRange(products);
    }

    public async Task SoftDeleteProductsAsync(int userId)
    {
        await _context.Products
            .Where(p => p.UserId == userId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(product => product.IsDeleted, true)
            );
    }

    public void UpdateProduct(Product product)
    {
        _context.Products.Update(product);
    }

    public async Task<bool> ExistProductByName(string name)
    {
        return await _context.Products
            .AnyAsync(p => p.Name == name);
    }
    
    private static IQueryable<Product> WhereNotDeleted(IQueryable<Product> query)
    {
        return query.Where(p => !p.IsDeleted);
    }
}