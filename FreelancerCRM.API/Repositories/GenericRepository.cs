using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using FreelancerCRM.API.Data;
using FreelancerCRM.API.Repositories.Interfaces;

namespace FreelancerCRM.API.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly FreelancerCrmDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(FreelancerCrmDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // Basic CRUD Operations
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.SingleOrDefaultAsync(predicate);
    }

    // Pagination
    public virtual async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize)
    {
        return await _dbSet
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>> predicate)
    {
        return await _dbSet
            .Where(predicate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // Count Operations
    public virtual async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate);
    }

    // Existence Check
    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    // Add Operations
    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    // Update Operations
    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
        await Task.CompletedTask;
    }

    // Delete Operations
    public virtual async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            await DeleteAsync(entity);
        }
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
        await Task.CompletedTask;
    }

    // Include Operations (for navigation properties)
    public virtual async Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, GetPrimaryKeyName()) == id);
    }

    public virtual async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.Where(predicate).ToListAsync();
    }

    // Helper method to get primary key name
    protected virtual string GetPrimaryKeyName()
    {
        var entityType = _context.Model.FindEntityType(typeof(T));
        var primaryKey = entityType?.FindPrimaryKey();
        return primaryKey?.Properties.FirstOrDefault()?.Name ?? "Id";
    }
} 