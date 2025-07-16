using AutoMapper;
using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FreelancerCRM.API.Services;

public abstract class BaseService<TEntity, TCreateDto, TUpdateDto, TResponseDto, TListDto> : IBaseService<TEntity, TCreateDto, TUpdateDto, TResponseDto, TListDto>
    where TEntity : BaseEntity
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly ILogger<BaseService<TEntity, TCreateDto, TUpdateDto, TResponseDto, TListDto>> _logger;
    protected readonly IMapper _mapper;

    protected BaseService(
        IUnitOfWork unitOfWork, 
        ILogger<BaseService<TEntity, TCreateDto, TUpdateDto, TResponseDto, TListDto>> logger,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public virtual async Task<ServiceResult<TResponseDto>> GetByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return ServiceResult<TResponseDto>.ValidationFailure(new List<string> { "Id must be greater than 0" });
            }

            var entity = await GetEntityByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<TResponseDto>.Failure($"Entity with id {id} not found");
            }

            var responseDto = _mapper.Map<TResponseDto>(entity);
            return ServiceResult<TResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity by id {Id}", id);
            return ServiceResult<TResponseDto>.Failure("An error occurred while retrieving the entity");
        }
    }

    public virtual async Task<ServiceResult<IEnumerable<TListDto>>> GetAllAsync()
    {
        try
        {
            var entities = await GetAllEntitiesAsync();
            var dtos = _mapper.Map<IEnumerable<TListDto>>(entities);
            return ServiceResult<IEnumerable<TListDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all entities");
            return ServiceResult<IEnumerable<TListDto>>.Failure("An error occurred while retrieving entities");
        }
    }

    public virtual async Task<ServiceResult<TResponseDto>> CreateAsync(TCreateDto createDto)
    {
        try
        {
            var entity = _mapper.Map<TEntity>(createDto);
            
            var validationResult = await ValidateEntityAsync(entity, false);
            if (!validationResult.IsValid)
            {
                return ServiceResult<TResponseDto>.ValidationFailure(validationResult.Errors);
            }

            await _unitOfWork.BeginTransactionAsync();
            var createdEntity = await CreateEntityAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            var responseDto = _mapper.Map<TResponseDto>(createdEntity);
            return ServiceResult<TResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error creating entity");
            return ServiceResult<TResponseDto>.Failure("An error occurred while creating the entity");
        }
    }

    public virtual async Task<ServiceResult<TResponseDto>> UpdateAsync(int id, TUpdateDto updateDto)
    {
        try
        {
            var entity = await GetEntityByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<TResponseDto>.Failure($"Entity with id {id} not found");
            }

            _mapper.Map(updateDto, entity);

            var validationResult = await ValidateEntityAsync(entity, true);
            if (!validationResult.IsValid)
            {
                return ServiceResult<TResponseDto>.ValidationFailure(validationResult.Errors);
            }

            await _unitOfWork.BeginTransactionAsync();
            await UpdateEntityAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            var responseDto = _mapper.Map<TResponseDto>(entity);
            return ServiceResult<TResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating entity with id {Id}", id);
            return ServiceResult<TResponseDto>.Failure("An error occurred while updating the entity");
        }
    }

    public virtual async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        try
        {
            var entity = await GetEntityByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<bool>.Failure($"Entity with id {id} not found");
            }

            var (canDelete, reason) = await CanDeleteEntityAsync(entity);
            if (!canDelete)
            {
                return ServiceResult<bool>.Failure(reason);
            }

            await _unitOfWork.BeginTransactionAsync();
            await DeleteEntityAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error deleting entity with id {Id}", id);
            return ServiceResult<bool>.Failure("An error occurred while deleting the entity");
        }
    }

    protected abstract Task<TEntity?> GetEntityByIdAsync(int id);
    protected abstract Task<IEnumerable<TEntity>> GetAllEntitiesAsync();
    protected abstract Task<TEntity> CreateEntityAsync(TEntity entity);
    protected abstract Task UpdateEntityAsync(TEntity entity);
    protected abstract Task DeleteEntityAsync(TEntity entity);
    protected abstract Task<bool> EntityExistsAsync(int id);
    protected abstract Task<ValidationResult> ValidateEntityAsync(TEntity entity, bool isUpdate);
    protected abstract Task<(bool CanDelete, string Reason)> CanDeleteEntityAsync(TEntity entity);
}

public class ValidationResult
{
    public bool IsValid { get; private set; }
    public List<string> Errors { get; private set; }

    private ValidationResult(bool isValid, List<string> errors)
    {
        IsValid = isValid;
        Errors = errors;
    }

    public static ValidationResult Success()
    {
        return new ValidationResult(true, new List<string>());
    }

    public static ValidationResult Failure(List<string> errors)
    {
        return new ValidationResult(false, errors);
    }

    public static ValidationResult Failure(string error)
    {
        return new ValidationResult(false, new List<string> { error });
    }
}