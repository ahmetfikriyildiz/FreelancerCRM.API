namespace FreelancerCRM.API.Services.Interfaces;

public interface IBaseService<T> where T : class
{
    Task<ServiceResult<T>> GetByIdAsync(int id);
    Task<ServiceResult<IEnumerable<T>>> GetAllAsync();
    Task<ServiceResult<T>> CreateAsync(T entity);
    Task<ServiceResult<T>> UpdateAsync(T entity);
    Task<ServiceResult<bool>> DeleteAsync(int id);
    Task<ServiceResult<bool>> ExistsAsync(int id);
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