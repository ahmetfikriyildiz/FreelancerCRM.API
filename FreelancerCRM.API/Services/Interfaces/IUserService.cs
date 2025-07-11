using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Services.Interfaces;

public interface IUserService : IBaseService<User>
{
    Task<ServiceResult<User>> GetByUsernameAsync(string username);
    Task<ServiceResult<User>> GetByEmailAsync(string email);
    Task<ServiceResult<User>> RegisterUserAsync(User user, string password);
    Task<ServiceResult<User>> AuthenticateAsync(string username, string password);
    Task<ServiceResult<bool>> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<ServiceResult<bool>> ResetPasswordAsync(string email);
    Task<ServiceResult<User>> UpdateProfileAsync(User user);
    Task<ServiceResult<IEnumerable<User>>> GetActiveUsersAsync();
    Task<ServiceResult<User>> GetUserWithRelationsAsync(int userId);
    Task<ServiceResult<bool>> DeactivateUserAsync(int userId);
    Task<ServiceResult<bool>> ActivateUserAsync(int userId);
    Task<ServiceResult<bool>> IsUsernameAvailableAsync(string username);
    Task<ServiceResult<bool>> IsEmailAvailableAsync(string email);
} 