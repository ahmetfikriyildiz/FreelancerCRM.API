using AutoMapper;
using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace FreelancerCRM.API.Services;

public class UserService : BaseService<User, UserCreateDto, UserUpdateDto, UserResponseDto, UserSummaryDto>, IUserService
{
    public UserService(
        IUnitOfWork unitOfWork,
        ILogger<UserService> logger,
        IMapper mapper)
        : base(unitOfWork, logger, mapper)
    {
    }

    #region BaseService Implementation
    protected override async Task<User?> GetEntityByIdAsync(int id)
    {
        return await _unitOfWork.Users.GetByIdAsync(id);
    }

    protected override async Task<IEnumerable<User>> GetAllEntitiesAsync()
    {
        return await _unitOfWork.Users.GetAllAsync();
    }

    protected override async Task<User> CreateEntityAsync(User entity)
    {
        return await _unitOfWork.Users.AddAsync(entity);
    }

    protected override async Task UpdateEntityAsync(User entity)
    {
        await _unitOfWork.Users.UpdateAsync(entity);
    }

    protected override async Task DeleteEntityAsync(User entity)
    {
        await _unitOfWork.Users.DeleteAsync(entity);
    }

    protected override async Task<bool> EntityExistsAsync(int id)
    {
        return await _unitOfWork.Users.ExistsAsync(u => u.Id == id);
    }

    protected override async Task<ValidationResult> ValidateEntityAsync(User entity, bool isUpdate)
    {
        var errors = new List<string>();

        // Required fields validation
        if (string.IsNullOrWhiteSpace(entity.FirstName))
            errors.Add("First name is required");

        if (string.IsNullOrWhiteSpace(entity.LastName))
            errors.Add("Last name is required");

        if (string.IsNullOrWhiteSpace(entity.Email))
            errors.Add("Email is required");
        else if (!IsValidEmail(entity.Email))
            errors.Add("Invalid email format");
        else if (!isUpdate)
        {
            // Check for duplicate email only on create
            var emailExists = await _unitOfWork.Users.IsEmailExistsAsync(entity.Email);
            if (emailExists)
                errors.Add("Email already exists");
        }

        if (!string.IsNullOrWhiteSpace(entity.Phone) && !IsValidPhone(entity.Phone))
            errors.Add("Invalid phone number format");

        await Task.CompletedTask;
        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    protected override async Task<(bool CanDelete, string Reason)> CanDeleteEntityAsync(User entity)
    {
        // Check for active projects
        var hasActiveProjects = await _unitOfWork.Projects.ExistsAsync(p =>
            p.UserID == entity.Id &&
            (p.Status == "InProgress" || p.Status == "Planning"));

        if (hasActiveProjects)
            return (false, "Cannot delete user with active projects");

        // Check for unpaid invoices
        var hasUnpaidInvoices = await _unitOfWork.Invoices.ExistsAsync(i =>
            i.UserID == entity.Id &&
            i.Status != "Paid" &&
            i.Status != "Cancelled");

        if (hasUnpaidInvoices)
            return (false, "Cannot delete user with unpaid invoices");

        return (true, string.Empty);
    }
    #endregion

    #region IUserService Implementation
    public override async Task<ServiceResult<UserResponseDto>> CreateAsync(UserCreateDto createDto)
    {
        try
        {
            // Validate password
            if (!IsValidPassword(createDto.Password))
            {
                return ServiceResult<UserResponseDto>.ValidationFailure(new List<string>
                {
                    "Password must be at least 8 characters long and contain uppercase, lowercase, number, and special character"
                });
            }

            // Map DTO to entity and set additional properties
            var user = _mapper.Map<User>(createDto);
            user.Username = createDto.Email;
            user.PasswordHash = HashPassword(createDto.Password);
            user.Role = UserRole.Freelancer; // Default role
            user.IsActive = true;

            // Validate entity
            var validationResult = await ValidateEntityAsync(user, false);
            if (!validationResult.IsValid)
            {
                return ServiceResult<UserResponseDto>.ValidationFailure(validationResult.Errors);
            }

            // Save to database
            await _unitOfWork.BeginTransactionAsync();
            var createdUser = await CreateEntityAsync(user);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            var responseDto = _mapper.Map<UserResponseDto>(createdUser);
            return ServiceResult<UserResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error creating user");
            return ServiceResult<UserResponseDto>.Failure("An error occurred while creating the user");
        }
    }

    public async Task<ServiceResult<UserResponseDto>> GetByEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                return ServiceResult<UserResponseDto>.ValidationFailure(new List<string> { "Email is required" });

            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
                return ServiceResult<UserResponseDto>.Failure("User not found");

            var responseDto = _mapper.Map<UserResponseDto>(user);
            return ServiceResult<UserResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email {Email}", email);
            return ServiceResult<UserResponseDto>.Failure("An error occurred while retrieving the user");
        }
    }

    public async Task<ServiceResult<UserResponseDto>> RegisterUserAsync(UserCreateDto createDto)
    {
        return await CreateAsync(createDto);
    }

    public async Task<ServiceResult<UserResponseDto>> AuthenticateAsync(UserLoginDto loginDto)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);
            if (user == null || !user.IsActive)
                return ServiceResult<UserResponseDto>.Failure("Invalid email or password");

            if (!VerifyPassword(loginDto.Password, user.PasswordHash))
                return ServiceResult<UserResponseDto>.Failure("Invalid email or password");

            var responseDto = _mapper.Map<UserResponseDto>(user);
            return ServiceResult<UserResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating user {Email}", loginDto.Email);
            return ServiceResult<UserResponseDto>.Failure("An error occurred during authentication");
        }
    }

    public async Task<ServiceResult<bool>> ChangePasswordAsync(UserChangePasswordDto changePasswordDto)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(changePasswordDto.UserId);
            if (user == null)
                return ServiceResult<bool>.Failure("User not found");

            if (!VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
                return ServiceResult<bool>.Failure("Current password is incorrect");

            if (!IsValidPassword(changePasswordDto.NewPassword))
                return ServiceResult<bool>.ValidationFailure(new List<string>
                {
                    "New password must be at least 8 characters long and contain uppercase, lowercase, number, and special character"
                });

            user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {Id}", changePasswordDto.UserId);
            return ServiceResult<bool>.Failure("An error occurred while changing the password");
        }
    }

    public async Task<ServiceResult<IEnumerable<UserSummaryDto>>> GetActiveUsersAsync()
    {
        try
        {
            var users = await _unitOfWork.Users.GetActiveUsersAsync();
            var userDtos = _mapper.Map<IEnumerable<UserSummaryDto>>(users);
            return ServiceResult<IEnumerable<UserSummaryDto>>.Success(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active users");
            return ServiceResult<IEnumerable<UserSummaryDto>>.Failure("An error occurred while retrieving active users");
        }
    }

    public async Task<ServiceResult<UserResponseDto>> GetUserWithRelationsAsync(int id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetUserWithAllRelationsAsync(id);
            if (user == null)
                return ServiceResult<UserResponseDto>.Failure("User not found");

            var responseDto = _mapper.Map<UserResponseDto>(user);
            return ServiceResult<UserResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user with relations {Id}", id);
            return ServiceResult<UserResponseDto>.Failure("An error occurred while retrieving the user");
        }
    }

    public async Task<ServiceResult<bool>> DeactivateUserAsync(int id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return ServiceResult<bool>.Failure("User not found");

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating user {Id}", id);
            return ServiceResult<bool>.Failure("An error occurred while deactivating the user");
        }
    }

    public async Task<ServiceResult<bool>> ActivateUserAsync(int id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return ServiceResult<bool>.Failure("User not found");

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating user {Id}", id);
            return ServiceResult<bool>.Failure("An error occurred while activating the user");
        }
    }

    public async Task<ServiceResult<bool>> IsUsernameAvailableAsync(string username)
    {
        try
        {
            var exists = await _unitOfWork.Users.IsUsernameExistsAsync(username);
            return ServiceResult<bool>.Success(!exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking username availability {Username}", username);
            return ServiceResult<bool>.Failure("An error occurred while checking username availability");
        }
    }

    public async Task<ServiceResult<bool>> IsEmailAvailableAsync(string email)
    {
        try
        {
            var exists = await _unitOfWork.Users.IsEmailExistsAsync(email);
            return ServiceResult<bool>.Success(!exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email availability {Email}", email);
            return ServiceResult<bool>.Failure("An error occurred while checking email availability");
        }
    }
    #endregion

    #region Helper Methods
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;

        bool hasUpperCase = password.Any(char.IsUpper);
        bool hasLowerCase = password.Any(char.IsLower);
        bool hasNumber = password.Any(char.IsDigit);
        bool hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

        return hasUpperCase && hasLowerCase && hasNumber && hasSpecialChar;
    }

    private bool IsValidPhone(string phone)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\+?[\d\s-()]+$");
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
    #endregion
} 