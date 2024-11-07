using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace OnlineMarketplace.Data.Repository;

public class EntityBaseRepository<T> : IEntityBaseRepository<T> where T : class, IEntityBase, new()
{
    private readonly ShopContext _context;
    private readonly DbSet<T> _table;

    public EntityBaseRepository(ShopContext context)
    {
        _context = context;
        _table = _context.Set<T>();
    }

    public async Task AddAsync(T entity) => await _table.AddAsync(entity);

    public async Task UpdateAsync(int id, T entity)
    {
        var existingEntity = await _table.FindAsync(id);

        if (existingEntity != null)
        {
            _context.Entry(existingEntity).CurrentValues.SetValues(entity); // Update only the modified fields
            _context.Entry(existingEntity).State = EntityState.Modified;  // Set state as Modified
        }
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _table.FindAsync(id);
        if (entity != null)
        {
            _table.Remove(entity);  // Mark the entity for deletion
        }
    }

    public async Task<IEnumerable<T>?> GetAllAsync() => await _table.ToListAsync();

    public async Task<IEnumerable<T>?> GetAllAsync(params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _table;
        query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<T>?> GetAllByFilterAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _table.Where(filter);
        
        if (includeProperties != null)
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id) => await _table.FindAsync(id);

    public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _table;
        query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        return await query.FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<T?> GetByFilterAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _table.Where(filter);

        if (includeProperties != null)
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        return await query.FirstOrDefaultAsync();
    }


    public async Task SaveAsync() => await _context.SaveChangesAsync();
}