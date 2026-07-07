using CustomerManagement.Domain.Common;
using CustomerManagement.Persistence.Data;
using CustomerManagement.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet
            .Where(entity => !entity.IsDeleted)
            .ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet
            .FirstOrDefaultAsync(entity => entity.Id == id && !entity.IsDeleted);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.UpdatedDate = DateTime.UtcNow;

        _dbSet.Update(entity);
    }
}