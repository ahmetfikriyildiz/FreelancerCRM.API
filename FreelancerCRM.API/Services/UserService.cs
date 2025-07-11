using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace FreelancerCRM.API.Services;

public class UserService : BaseService<User>, IUserService
{
    public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger) 
        : base(unitOfWork, logger)
    {
    }

    #region Base Service Implementation
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
        return await _unitOfWork.Users.ExistsAsync(u => u.UserID == id);
    }

    protected override async Task<ValidationResult> ValidateEntityAsync(User entity, bool isUpdate)
    {
        var errors = new List<string>();

        // Username validation
        if (string.IsNullOrWhiteSpace(entity.Username))
        {
            errors.Add("Username is required");
        }
        else if (entity.Username.Length < 3 || entity.Username.Length > 50)
        {
            errors.Add("Username must be between 3 and 50 characters");
        }
        else if (!Regex.IsMatch(entity.Username, @"^[a-zA-Z0-9_]+$"))
        {
            errors.Add("Username can only contain letters, numbers, and underscores");
        }
        else if (!isUpdate && await _unitOfWork.Users.IsUsernameExistsAsync(entity.Username))
        {
            errors.Add("Username already exists");
        }

        // Email validation
        if (string.IsNullOrWhiteSpace(entity.Email))
        {
            errors.Add("Email is required");
        }
        else if (!IsValidEmail(entity.Email))
        {
            errors.Add("Invalid email format");
        }
        else if (!isUpdate && await _unitOfWork.Users.IsEmailExistsAsync(entity.Email))
        {
            errors.Add("Email already exists");
        }

        // Name validation
        if (string.IsNullOrWhiteSpace(entity.FirstName))
        {
            errors.Add("First name is required");
        }
        else if (entity.FirstName.Length > 100)
        {
            errors.Add("First name cannot exceed 100 characters");
        }

        if (string.IsNullOrWhiteSpace(entity.LastName))
        {
            errors.Add("Last name is required");
        }
        else if (entity.LastName.Length > 100)
        {
            errors.Add("Last name cannot exceed 100 characters");
        }

        // TCKN validation (Turkish ID Number)
        if (!string.IsNullOrWhiteSpace(entity.TCKN) && !IsValidTCKN(entity.TCKN))
        {
            errors.Add("Invalid TCKN format");
        }

        // Tax Number validation
        if (!string.IsNullOrWhiteSpace(entity.TaxNumber) && !IsValidTaxNumber(entity.TaxNumber))
        {
            errors.Add("Invalid tax number format");
        }

        // Phone validation
        if (!string.IsNullOrWhiteSpace(entity.Phone) && !IsValidPhone(entity.Phone))
        {
            errors.Add("Invalid phone number format");
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    protected override async Task<(bool CanDelete, string Reason)> CanDeleteEntityAsync(User entity)
    {
        // Check if user has active clients
        var clientCount = await _unitOfWork.Clients.CountAsync(c => c.UserID == entity.UserID);
        if (clientCount > 0)
        {
            return (false, "Cannot delete user with existing clients");
        }

        // Check if user has active projects
        var projectCount = await _unitOfWork.Projects.CountAsync(p => p.UserID == entity.UserID);
        if (projectCount > 0)
        {
            return (false, "Cannot delete user with existing projects");
        }

        return (true, string.Empty);
    }
    #endregion

    #region User-Specific Methods
    public async Task<ServiceResult<User>> GetByUsernameAsync(string username)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return ServiceResult<User>.ValidationFailure(new List<string> { "Username is required" });
            }

            var user = await _unitOfWork.Users.GetByUsernameAsync(username);
            if (user == null)
            {
                return ServiceResult<User>.Failure("User not found");
            }

            return ServiceResult<User>.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by username {Username}", username);
            return ServiceResult<User>.Failure("An error occurred while retrieving the user");
        }
    }

    public async Task<ServiceResult<User>> GetByEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return ServiceResult<User>.ValidationFailure(new List<string> { "Email is required" });
            }

            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
            {
                return ServiceResult<User>.Failure("User not found");
            }

            return ServiceResult<User>.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email {Email}", email);
            return ServiceResult<User>.Failure("An error occurred while retrieving the user");
        }
    }

    public async Task<ServiceResult<User>> RegisterUserAsync(User user, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return ServiceResult<User>.ValidationFailure(new List<string> { "Password is required" });
            }

            if (!IsValidPassword(password))
            {
                return ServiceResult<User>.ValidationFailure(new List<string> { "Password must be at least 8 characters long and contain uppercase, lowercase, number, and special character" });
            }

            user.PasswordHash = HashPassword(password);
            user.IsActive = true;
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            return await CreateAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            return ServiceResult<User>.Failure("An error occurred while registering the user");
        }
    }

    public async Task<ServiceResult<User>> AuthenticateAsync(string username, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return ServiceResult<User>.ValidationFailure(new List<string> { "Username and password are required" });
            }

            var user = await _unitOfWork.Users.GetByUsernameAsync(username);
            if (user == null || !user.IsActive)
            {
                return ServiceResult<User>.Failure("Invalid username or password");
            }

            if (!VerifyPassword(password, user.PasswordHash))
            {
                return ServiceResult<User>.Failure("Invalid username or password");
            }

            return ServiceResult<User>.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating user {Username}", username);
            return ServiceResult<User>.Failure("An error occurred during authentication");
        }
    }

    public async Task<ServiceResult<bool>> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return ServiceResult<bool>.Failure("User not found");
            }

            if (!VerifyPassword(currentPassword, user.PasswordHash))
            {
                return ServiceResult<bool>.Failure("Current password is incorrect");
            }

            if (!IsValidPassword(newPassword))
            {
                return ServiceResult<bool>.ValidationFailure(new List<string> { "New password must be at least 8 characters long and contain uppercase, lowercase, number, and special character" });
            }

            await _unitOfWork.BeginTransactionAsync();

            user.PasswordHash = HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error changing password for user {UserId}", userId);
            return ServiceResult<bool>.Failure("An error occurred while changing the password");
        }
    }

    public async Task<ServiceResult<bool>> ResetPasswordAsync(string email)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal if email exists or not for security
                return ServiceResult<bool>.Success(true);
            }

            // Generate temporary password
            var tempPassword = GenerateTemporaryPassword();
            user.PasswordHash = HashPassword(tempPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            // TODO: Send email with temporary password
            _logger.LogInformation("Password reset for user {Email}", email);

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error resetting password for email {Email}", email);
            return ServiceResult<bool>.Failure("An error occurred while resetting the password");
        }
    }

    public async Task<ServiceResult<User>> UpdateProfileAsync(User user)
    {
        try
        {
            var existingUser = await _unitOfWork.Users.GetByIdAsync(user.UserID);
            if (existingUser == null)
            {
                return ServiceResult<User>.Failure("User not found");
            }

            // Update only profile fields
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;
            existingUser.ProfilePicture = user.ProfilePicture;
            existingUser.Timezone = user.Timezone;
            existingUser.TaxNumber = user.TaxNumber;
            existingUser.TaxOffice = user.TaxOffice;
            existingUser.TCKN = user.TCKN;
            existingUser.IsKDVMukellefi = user.IsKDVMukellefi;
            existingUser.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(existingUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user {UserId}", user.UserID);
            return ServiceResult<User>.Failure("An error occurred while updating the profile");
        }
    }

    public async Task<ServiceResult<IEnumerable<User>>> GetActiveUsersAsync()
    {
        try
        {
            var users = await _unitOfWork.Users.GetActiveUsersAsync();
            return ServiceResult<IEnumerable<User>>.Success(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active users");
            return ServiceResult<IEnumerable<User>>.Failure("An error occurred while retrieving active users");
        }
    }

    public async Task<ServiceResult<User>> GetUserWithRelationsAsync(int userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetUserWithAllRelationsAsync(userId);
            if (user == null)
            {
                return ServiceResult<User>.Failure("User not found");
            }

            return ServiceResult<User>.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user with relations {UserId}", userId);
            return ServiceResult<User>.Failure("An error occurred while retrieving the user");
        }
    }

    public async Task<ServiceResult<bool>> DeactivateUserAsync(int userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return ServiceResult<bool>.Failure("User not found");
            }

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error deactivating user {UserId}", userId);
            return ServiceResult<bool>.Failure("An error occurred while deactivating the user");
        }
    }

    public async Task<ServiceResult<bool>> ActivateUserAsync(int userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return ServiceResult<bool>.Failure("User not found");
            }

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error activating user {UserId}", userId);
            return ServiceResult<bool>.Failure("An error occurred while activating the user");
        }
    }

    public async Task<ServiceResult<bool>> IsUsernameAvailableAsync(string username)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return ServiceResult<bool>.ValidationFailure(new List<string> { "Username is required" });
            }

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
            if (string.IsNullOrWhiteSpace(email))
            {
                return ServiceResult<bool>.ValidationFailure(new List<string> { "Email is required" });
            }

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
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private bool IsValidPassword(string password)
    {
        return password.Length >= 8 &&
               Regex.IsMatch(password, @"[A-Z]") &&
               Regex.IsMatch(password, @"[a-z]") &&
               Regex.IsMatch(password, @"[0-9]") &&
               Regex.IsMatch(password, @"[^a-zA-Z0-9]");
    }

    private bool IsValidTCKN(string tckn)
    {
        if (string.IsNullOrWhiteSpace(tckn) || tckn.Length != 11)
            return false;

        return Regex.IsMatch(tckn, @"^\d{11}$");
    }

    private bool IsValidTaxNumber(string taxNumber)
    {
        if (string.IsNullOrWhiteSpace(taxNumber))
            return false;

        return Regex.IsMatch(taxNumber, @"^\d{10}$");
    }

    private bool IsValidPhone(string phone)
    {
        return Regex.IsMatch(phone, @"^(\+90|0)?[1-9]\d{9}$");
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string hash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == hash;
    }

    private string GenerateTemporaryPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    #endregion
} 