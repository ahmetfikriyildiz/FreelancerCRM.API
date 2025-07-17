using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Services.Interfaces;

/// <summary>
/// Kullanıcı işlemlerini yöneten servis arayüzü
/// </summary>
public interface IUserService : IBaseService<User, UserCreateDto, UserUpdateDto, UserResponseDto, UserSummaryDto>
{
    Task<ServiceResult<UserResponseDto>> GetByEmailAsync(string email);
    Task<ServiceResult<UserResponseDto>> RegisterUserAsync(UserCreateDto createDto);
    Task<ServiceResult<UserResponseDto>> AuthenticateAsync(UserLoginDto loginDto);
    Task<ServiceResult<bool>> ChangePasswordAsync(UserChangePasswordDto changePasswordDto);
    Task<ServiceResult<IEnumerable<UserSummaryDto>>> GetActiveUsersAsync();
    Task<ServiceResult<UserResponseDto>> GetUserWithRelationsAsync(int id);
    Task<ServiceResult<bool>> DeactivateUserAsync(int id);
    Task<ServiceResult<bool>> ActivateUserAsync(int id);
    Task<ServiceResult<bool>> IsUsernameAvailableAsync(string username);
    Task<ServiceResult<bool>> IsEmailAvailableAsync(string email);
} 