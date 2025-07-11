using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace FreelancerCRM.API.Services;

public class ClientService : BaseService<Client>, IClientService
{
    public ClientService(IUnitOfWork unitOfWork, ILogger<ClientService> logger) 
        : base(unitOfWork, logger)
    {
    }

    #region Base Service Implementation
    protected override async Task<Client?> GetEntityByIdAsync(int id)
    {
        return await _unitOfWork.Clients.GetByIdAsync(id);
    }

    protected override async Task<IEnumerable<Client>> GetAllEntitiesAsync()
    {
        return await _unitOfWork.Clients.GetAllAsync();
    }

    protected override async Task<Client> CreateEntityAsync(Client entity)
    {
        return await _unitOfWork.Clients.AddAsync(entity);
    }

    protected override async Task UpdateEntityAsync(Client entity)
    {
        await _unitOfWork.Clients.UpdateAsync(entity);
    }

    protected override async Task DeleteEntityAsync(Client entity)
    {
        await _unitOfWork.Clients.DeleteAsync(entity);
    }

    protected override async Task<bool> EntityExistsAsync(int id)
    {
        return await _unitOfWork.Clients.ExistsAsync(c => c.ClientID == id);
    }

    protected override async Task<ValidationResult> ValidateEntityAsync(Client entity, bool isUpdate)
    {
        var errors = new List<string>();

        // Company name validation
        if (string.IsNullOrWhiteSpace(entity.CompanyName))
        {
            errors.Add("Company name is required");
        }
        else if (entity.CompanyName.Length > 255)
        {
            errors.Add("Company name cannot exceed 255 characters");
        }

        // Email validation
        if (!string.IsNullOrWhiteSpace(entity.Email) && !IsValidEmail(entity.Email))
        {
            errors.Add("Invalid email format");
        }

        // Phone validation
        if (!string.IsNullOrWhiteSpace(entity.Phone) && !IsValidPhone(entity.Phone))
        {
            errors.Add("Invalid phone number format");
        }

        // Tax number validation
        if (!string.IsNullOrWhiteSpace(entity.TaxNumber) && !IsValidTaxNumber(entity.TaxNumber))
        {
            errors.Add("Invalid tax number format");
        }

        // Priority validation
        if (!string.IsNullOrWhiteSpace(entity.Priority) && 
            !new[] { "High", "Medium", "Low" }.Contains(entity.Priority))
        {
            errors.Add("Priority must be High, Medium, or Low");
        }

        // Status validation
        if (!string.IsNullOrWhiteSpace(entity.Status) && 
            !new[] { "Active", "Inactive", "Archived" }.Contains(entity.Status))
        {
            errors.Add("Status must be Active, Inactive, or Archived");
        }

        await Task.CompletedTask;
        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    protected override async Task<(bool CanDelete, string Reason)> CanDeleteEntityAsync(Client entity)
    {
        // Check if client has active projects
        var projectCount = await _unitOfWork.Projects.CountAsync(p => p.ClientID == entity.ClientID);
        if (projectCount > 0)
        {
            return (false, "Cannot delete client with existing projects");
        }

        // Check if client has invoices
        var invoiceCount = await _unitOfWork.Invoices.CountAsync(i => i.ClientID == entity.ClientID);
        if (invoiceCount > 0)
        {
            return (false, "Cannot delete client with existing invoices");
        }

        return (true, string.Empty);
    }
    #endregion

    #region Client-Specific Methods
    public async Task<ServiceResult<IEnumerable<Client>>> GetClientsByUserIdAsync(int userId)
    {
        try
        {
            var clients = await _unitOfWork.Clients.GetClientsByUserIdAsync(userId);
            return ServiceResult<IEnumerable<Client>>.Success(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting clients for user {UserId}", userId);
            return ServiceResult<IEnumerable<Client>>.Failure("An error occurred while retrieving clients");
        }
    }

    public async Task<ServiceResult<IEnumerable<Client>>> GetActiveClientsAsync()
    {
        try
        {
            var clients = await _unitOfWork.Clients.GetActiveClientsAsync();
            return ServiceResult<IEnumerable<Client>>.Success(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active clients");
            return ServiceResult<IEnumerable<Client>>.Failure("An error occurred while retrieving active clients");
        }
    }

    public async Task<ServiceResult<IEnumerable<Client>>> GetClientsByStatusAsync(string status)
    {
        try
        {
            var clients = await _unitOfWork.Clients.GetClientsByStatusAsync(status);
            return ServiceResult<IEnumerable<Client>>.Success(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting clients by status {Status}", status);
            return ServiceResult<IEnumerable<Client>>.Failure("An error occurred while retrieving clients");
        }
    }

    public async Task<ServiceResult<IEnumerable<Client>>> SearchClientsAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return ServiceResult<IEnumerable<Client>>.ValidationFailure(new List<string> { "Search term is required" });
            }

            var clients = await _unitOfWork.Clients.SearchClientsAsync(searchTerm);
            return ServiceResult<IEnumerable<Client>>.Success(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching clients with term {SearchTerm}", searchTerm);
            return ServiceResult<IEnumerable<Client>>.Failure("An error occurred while searching clients");
        }
    }

    public async Task<ServiceResult<Client>> GetClientWithProjectsAsync(int clientId)
    {
        try
        {
            var client = await _unitOfWork.Clients.GetClientWithProjectsAsync(clientId);
            if (client == null)
            {
                return ServiceResult<Client>.Failure("Client not found");
            }

            return ServiceResult<Client>.Success(client);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client with projects {ClientId}", clientId);
            return ServiceResult<Client>.Failure("An error occurred while retrieving client");
        }
    }

    public async Task<ServiceResult<Client>> GetClientWithInvoicesAsync(int clientId)
    {
        try
        {
            var client = await _unitOfWork.Clients.GetClientWithInvoicesAsync(clientId);
            if (client == null)
            {
                return ServiceResult<Client>.Failure("Client not found");
            }

            return ServiceResult<Client>.Success(client);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client with invoices {ClientId}", clientId);
            return ServiceResult<Client>.Failure("An error occurred while retrieving client");
        }
    }

    public async Task<ServiceResult<Client>> GetClientWithAllRelationsAsync(int clientId)
    {
        try
        {
            var client = await _unitOfWork.Clients.GetClientWithAllRelationsAsync(clientId);
            if (client == null)
            {
                return ServiceResult<Client>.Failure("Client not found");
            }

            return ServiceResult<Client>.Success(client);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client with all relations {ClientId}", clientId);
            return ServiceResult<Client>.Failure("An error occurred while retrieving client");
        }
    }

    public async Task<ServiceResult<bool>> ArchiveClientAsync(int clientId)
    {
        try
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(clientId);
            if (client == null)
            {
                return ServiceResult<bool>.Failure("Client not found");
            }

            client.Status = "Archived";
            client.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Clients.UpdateAsync(client);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error archiving client {ClientId}", clientId);
            return ServiceResult<bool>.Failure("An error occurred while archiving the client");
        }
    }

    public async Task<ServiceResult<bool>> UnarchiveClientAsync(int clientId)
    {
        try
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(clientId);
            if (client == null)
            {
                return ServiceResult<bool>.Failure("Client not found");
            }

            client.Status = "Active";
            client.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Clients.UpdateAsync(client);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error unarchiving client {ClientId}", clientId);
            return ServiceResult<bool>.Failure("An error occurred while unarchiving the client");
        }
    }

    public async Task<ServiceResult<bool>> SetPriorityAsync(int clientId, string priority)
    {
        try
        {
            if (!new[] { "High", "Medium", "Low" }.Contains(priority))
            {
                return ServiceResult<bool>.ValidationFailure(new List<string> { "Priority must be High, Medium, or Low" });
            }

            var client = await _unitOfWork.Clients.GetByIdAsync(clientId);
            if (client == null)
            {
                return ServiceResult<bool>.Failure("Client not found");
            }

            client.Priority = priority;
            client.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Clients.UpdateAsync(client);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error setting priority for client {ClientId}", clientId);
            return ServiceResult<bool>.Failure("An error occurred while setting client priority");
        }
    }

    public async Task<ServiceResult<bool>> ValidateTaxNumberAsync(string taxNumber)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(taxNumber))
            {
                return ServiceResult<bool>.ValidationFailure(new List<string> { "Tax number is required" });
            }

            var isValid = IsValidTaxNumber(taxNumber);
            await Task.CompletedTask;
            return ServiceResult<bool>.Success(isValid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating tax number {TaxNumber}", taxNumber);
            return ServiceResult<bool>.Failure("An error occurred while validating tax number");
        }
    }

    public async Task<ServiceResult<IEnumerable<Client>>> GetClientsByIndustryAsync(string industry)
    {
        try
        {
            var clients = await _unitOfWork.Clients.GetClientsByIndustryAsync(industry);
            return ServiceResult<IEnumerable<Client>>.Success(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting clients by industry {Industry}", industry);
            return ServiceResult<IEnumerable<Client>>.Failure("An error occurred while retrieving clients");
        }
    }

    public async Task<ServiceResult<IEnumerable<Client>>> GetClientsByCityAsync(string city)
    {
        try
        {
            var clients = await _unitOfWork.Clients.GetClientsByCityAsync(city);
            return ServiceResult<IEnumerable<Client>>.Success(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting clients by city {City}", city);
            return ServiceResult<IEnumerable<Client>>.Failure("An error occurred while retrieving clients");
        }
    }
    #endregion

    #region Helper Methods
    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private bool IsValidPhone(string phone)
    {
        return Regex.IsMatch(phone, @"^(\+90|0)?[1-9]\d{9}$");
    }

    private bool IsValidTaxNumber(string taxNumber)
    {
        if (string.IsNullOrWhiteSpace(taxNumber))
            return false;

        return Regex.IsMatch(taxNumber, @"^\d{10}$");
    }
    #endregion
} 