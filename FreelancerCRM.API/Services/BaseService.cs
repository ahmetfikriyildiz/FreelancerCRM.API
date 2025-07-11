using FreelancerCRM.API.Repositories.Interfaces;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FreelancerCRM.API.Services;

public abstract class BaseService<T> : IBaseService<T> where T : class
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly ILogger<BaseService<T>> _logger;

    protected BaseService(IUnitOfWork unitOfWork, ILogger<BaseService<T>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public virtual async Task<ServiceResult<T>> GetByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return ServiceResult<T>.ValidationFailure(new List<string> { "Id must be greater than 0" });
            }

            var entity = await GetEntityByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<T>.Failure($"Entity with id {id} not found");
            }

            return ServiceResult<T>.Success(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity by id {Id}", id);
            return ServiceResult<T>.Failure("An error occurred while retrieving the entity");
        }
    }

    public virtual async Task<ServiceResult<IEnumerable<T>>> GetAllAsync()
    {
        try
        {
            var entities = await GetAllEntitiesAsync();
            return ServiceResult<IEnumerable<T>>.Success(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all entities");
            return ServiceResult<IEnumerable<T>>.Failure("An error occurred while retrieving entities");
        }
    }

    public virtual async Task<ServiceResult<T>> CreateAsync(T entity)
    {
        try
        {
            var validationResult = await ValidateEntityAsync(entity, isUpdate: false);
            if (!validationResult.IsValid)
            {
                return ServiceResult<T>.ValidationFailure(validationResult.Errors);
            }

            await _unitOfWork.BeginTransactionAsync();

            await SetCreatedAtAsync(entity);
            await SetUpdatedAtAsync(entity);

            var createdEntity = await CreateEntityAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Entity created successfully");
            return ServiceResult<T>.Success(createdEntity);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error creating entity");
            return ServiceResult<T>.Failure("An error occurred while creating the entity");
        }
    }

    public virtual async Task<ServiceResult<T>> UpdateAsync(T entity)
    {
        try
        {
            var validationResult = await ValidateEntityAsync(entity, isUpdate: true);
            if (!validationResult.IsValid)
            {
                return ServiceResult<T>.ValidationFailure(validationResult.Errors);
            }

            await _unitOfWork.BeginTransactionAsync();

            await SetUpdatedAtAsync(entity);

            await UpdateEntityAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Entity updated successfully");
            return ServiceResult<T>.Success(entity);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating entity");
            return ServiceResult<T>.Failure("An error occurred while updating the entity");
        }
    }

    public virtual async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return ServiceResult<bool>.ValidationFailure(new List<string> { "Id must be greater than 0" });
            }

            var entity = await GetEntityByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<bool>.Failure($"Entity with id {id} not found");
            }

            var canDelete = await CanDeleteEntityAsync(entity);
            if (!canDelete.CanDelete)
            {
                return ServiceResult<bool>.Failure(canDelete.Reason);
            }

            await _unitOfWork.BeginTransactionAsync();

            await DeleteEntityAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Entity deleted successfully");
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error deleting entity with id {Id}", id);
            return ServiceResult<bool>.Failure("An error occurred while deleting the entity");
        }
    }

    public virtual async Task<ServiceResult<bool>> ExistsAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return ServiceResult<bool>.ValidationFailure(new List<string> { "Id must be greater than 0" });
            }

            var exists = await EntityExistsAsync(id);
            return ServiceResult<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if entity exists with id {Id}", id);
            return ServiceResult<bool>.Failure("An error occurred while checking entity existence");
        }
    }

    // Abstract methods to be implemented by derived classes
    protected abstract Task<T?> GetEntityByIdAsync(int id);
    protected abstract Task<IEnumerable<T>> GetAllEntitiesAsync();
    protected abstract Task<T> CreateEntityAsync(T entity);
    protected abstract Task UpdateEntityAsync(T entity);
    protected abstract Task DeleteEntityAsync(T entity);
    protected abstract Task<bool> EntityExistsAsync(int id);
    protected abstract Task<ValidationResult> ValidateEntityAsync(T entity, bool isUpdate);
    protected abstract Task<(bool CanDelete, string Reason)> CanDeleteEntityAsync(T entity);

    // Helper methods
    protected virtual async Task SetCreatedAtAsync(T entity)
    {
        var property = typeof(T).GetProperty("CreatedAt");
        if (property != null && property.PropertyType == typeof(DateTime))
        {
            property.SetValue(entity, DateTime.UtcNow);
        }
        await Task.CompletedTask;
    }

    protected virtual async Task SetUpdatedAtAsync(T entity)
    {
        var property = typeof(T).GetProperty("UpdatedAt");
        if (property != null && property.PropertyType == typeof(DateTime))
        {
            property.SetValue(entity, DateTime.UtcNow);
        }
        await Task.CompletedTask;
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ValidationResult Success()
    {
        return new ValidationResult { IsValid = true };
    }

    public static ValidationResult Failure(List<string> errors)
    {
        return new ValidationResult { IsValid = false, Errors = errors };
    }

    public static ValidationResult Failure(string error)
    {
        return new ValidationResult { IsValid = false, Errors = new List<string> { error } };
    }
} 