using System.Linq.Expressions;

namespace UserManagement.Domain.Repositories;

public interface IGenericRepository<T, K> where T : class
{
    Task<T> GetByIdAsync(K id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}