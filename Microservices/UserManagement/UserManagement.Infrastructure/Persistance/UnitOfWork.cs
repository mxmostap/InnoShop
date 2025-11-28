using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Repositories;

namespace UserManagement.Infrastructure.Persistance;

public class UnitOfWork : IUnitOfWork
{
    private readonly EFDBContext _context;
    public IUserRepository Users { get; }
    public IProfileRepository Profiles { get; }

    public UnitOfWork(
        EFDBContext context,
        IUserRepository users,
        IProfileRepository profiles)
    {
        _context = context;
        Users = users;
        Profiles = profiles;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public void DetachEntity<T>(T entity) where T : class
    {
        _context.Entry(entity).State = EntityState.Detached;
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}