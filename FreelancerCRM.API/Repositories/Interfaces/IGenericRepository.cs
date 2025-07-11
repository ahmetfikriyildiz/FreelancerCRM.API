using System.Linq.Expressions;

namespace FreelancerCRM.API.Repositories.Interfaces;

public interface IGenericRepository<T> where T : class
{
    // Basic CRUD Operations
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
    
    // Pagination
    Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize);
    Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>> predicate);
    
    // Count Operations
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    
    // Existence Check
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    
    // Add Operations
    Task<T> AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    
    // Update Operations
    Task UpdateAsync(T entity);
    Task UpdateRangeAsync(IEnumerable<T> entities);
    
    // Delete Operations
    Task DeleteAsync(T entity);
    Task DeleteAsync(int id);
    Task DeleteRangeAsync(IEnumerable<T> entities);
    
    // Include Operations (for navigation properties)
    Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
} 