namespace UserManagement.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IProfileRepository Profiles { get; }

    Task<int> SaveChangesAsync();
    void DetachEntity<T>(T entity) where T : class;
}