using Infomatrix.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Infomatrix.Api.Data;

public interface IRepository<T>
    where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default);

    void Update(T entity);

    void Delete(T entity);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}

public class Repository<T> : IRepository<T>
    where T : BaseEntity
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _dbSet
            .FindAsync([id], cancellationToken);

        return result;
    }

    public async Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        await _dbSet
            .AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context
            .SaveChangesAsync(cancellationToken);
    }
}
