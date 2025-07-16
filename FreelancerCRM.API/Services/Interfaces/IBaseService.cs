using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Services.Interfaces;

/// <summary>
/// Temel CRUD operasyonları için base service interface
/// </summary>
/// <typeparam name="TEntity">Veritabanı entity tipi</typeparam>
/// <typeparam name="TCreateDto">Create DTO tipi</typeparam>
/// <typeparam name="TUpdateDto">Update DTO tipi</typeparam>
/// <typeparam name="TResponseDto">Response DTO tipi</typeparam>
/// <typeparam name="TListDto">Liste response DTO tipi</typeparam>
public interface IBaseService<TEntity, TCreateDto, TUpdateDto, TResponseDto, TListDto> 
    where TEntity : BaseEntity
{
    Task<ServiceResult<TResponseDto>> GetByIdAsync(int id);
    Task<ServiceResult<IEnumerable<TListDto>>> GetAllAsync();
    Task<ServiceResult<TResponseDto>> CreateAsync(TCreateDto createDto);
    Task<ServiceResult<TResponseDto>> UpdateAsync(int id, TUpdateDto updateDto);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}

public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> ValidationErrors { get; set; } = new();

    public static ServiceResult<T> Success(T data)
    {
        return new ServiceResult<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    public static ServiceResult<T> Failure(string errorMessage)
    {
        return new ServiceResult<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }

    public static ServiceResult<T> ValidationFailure(List<string> validationErrors)
    {
        return new ServiceResult<T>
        {
            IsSuccess = false,
            ValidationErrors = validationErrors
        };
    }
} 