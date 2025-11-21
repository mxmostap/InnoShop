using ProductManagement.Microservice.Domain.Entities;

namespace ProductManagement.Microservice.Domain.Repositories;

public interface IProductRepository : IGenericRepository<Product, int>
{
    //Task<Product> GetByIdAsync(int productId);
    //Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Product>> GetAllAvailableAsync();
}