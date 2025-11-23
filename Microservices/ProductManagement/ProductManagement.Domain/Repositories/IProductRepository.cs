using ProductManagement.Microservice.Domain.Entities;

namespace ProductManagement.Microservice.Domain.Repositories;

public interface IProductRepository
{
    Task<Product> GetByIdAsync(int productId, int? userId = null);
    Task<IEnumerable<Product>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetAllAvailableAsync();
    Task AddAsync(Product product);
    void DeleteProduct(Product product);
    Task DeleteProductsByUserIdAsync(int userId);
    Task SoftDeleteProductsAsync(int userId);
    void UpdateProduct(Product product);
    Task<bool> ExistProductByName(string name);
}