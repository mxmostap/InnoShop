namespace ProductManagement.Microservice.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }

    Task<int> SaveChangesAsync();
}