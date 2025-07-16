using AutoMapper;
using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace FreelancerCRM.API.Services;

public class ClientService : BaseService<Client, ClientCreateDto, ClientUpdateDto, ClientResponseDto, ClientSummaryDto>, IClientService
{
    public ClientService(
        IUnitOfWork unitOfWork,
        ILogger<ClientService> logger,
        IMapper mapper)
        : base(unitOfWork, logger, mapper)
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
        if (!string.IsNullOrWhiteSpace(entity.Email))
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(entity.Email))
            {
                errors.Add("Invalid email format");
            }
        }

        // Phone validation
        if (!string.IsNullOrWhiteSpace(entity.Phone))
        {
            var phoneRegex = new Regex(@"^\+?[\d\s-]{10,}$");
            if (!phoneRegex.IsMatch(entity.Phone))
            {
                errors.Add("Invalid phone number format");
            }
        }

        // Website validation
        if (!string.IsNullOrWhiteSpace(entity.Website))
        {
            var websiteRegex = new Regex(@"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$");
            if (!websiteRegex.IsMatch(entity.Website))
            {
                errors.Add("Invalid website URL format");
            }
        }

        await Task.CompletedTask;
        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    protected override async Task<(bool CanDelete, string Reason)> CanDeleteEntityAsync(Client entity)
    {
        // Check if client has any projects
        var hasProjects = await _unitOfWork.Projects.ExistsAsync(p => p.ClientID == entity.ClientID);
        if (hasProjects)
        {
            return (false, "Cannot delete client with existing projects");
        }

        // Check if client has any invoices
        var hasInvoices = await _unitOfWork.Invoices.ExistsAsync(i => i.ClientID == entity.ClientID);
        if (hasInvoices)
        {
            return (false, "Cannot delete client with existing invoices");
        }

        return (true, string.Empty);
    }
    #endregion

    #region Interface Implementation
    public async Task<ServiceResult<IEnumerable<Client>>> GetClientsByUserIdAsync(int userId)
    {
        try
        {
            var clients = await _unitOfWork.Clients.FindAsync(c => c.UserID == userId);
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
            var clients = await _unitOfWork.Clients.FindAsync(c => c.Status != "Inactive" && !c.IsArchived);
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
            var clients = await _unitOfWork.Clients.FindAsync(c => c.Status == status);
            return ServiceResult<IEnumerable<Client>>.Success(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting clients with status {Status}", status);
            return ServiceResult<IEnumerable<Client>>.Failure("An error occurred while retrieving clients");
        }
    }

    public async Task<ServiceResult<IEnumerable<Client>>> GetClientsByIndustryAsync(string industry)
    {
        try
        {
            var clients = await _unitOfWork.Clients.FindAsync(c => c.Industry == industry);
            return ServiceResult<IEnumerable<Client>>.Success(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting clients in industry {Industry}", industry);
            return ServiceResult<IEnumerable<Client>>.Failure("An error occurred while retrieving clients");
        }
    }

    public async Task<ServiceResult<IEnumerable<Client>>> GetClientsByCityAsync(string city)
    {
        try
        {
            var clients = await _unitOfWork.Clients.FindAsync(c => c.City == city);
            return ServiceResult<IEnumerable<Client>>.Success(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting clients in city {City}", city);
            return ServiceResult<IEnumerable<Client>>.Failure("An error occurred while retrieving clients");
        }
    }

    public async Task<ServiceResult<Client>> GetClientWithProjectsAsync(int clientId)
    {
        try
        {
            var client = await _unitOfWork.Clients.GetByIdWithIncludesAsync(
                clientId,
                c => c.Projects!);

            if (client == null)
            {
                return ServiceResult<Client>.Failure($"Client with id {clientId} not found");
            }

            return ServiceResult<Client>.Success(client);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client with projects {ClientId}", clientId);
            return ServiceResult<Client>.Failure("An error occurred while retrieving the client");
        }
    }

    public async Task<ServiceResult<Client>> GetClientWithInvoicesAsync(int clientId)
    {
        try
        {
            var client = await _unitOfWork.Clients.GetByIdWithIncludesAsync(
                clientId,
                c => c.Invoices!);

            if (client == null)
            {
                return ServiceResult<Client>.Failure($"Client with id {clientId} not found");
            }

            return ServiceResult<Client>.Success(client);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client with invoices {ClientId}", clientId);
            return ServiceResult<Client>.Failure("An error occurred while retrieving the client");
        }
    }

    public async Task<ServiceResult<Client>> GetClientWithAllRelationsAsync(int clientId)
    {
        try
        {
            var client = await _unitOfWork.Clients.GetByIdWithIncludesAsync(
                clientId,
                c => c.Projects!,
                c => c.Invoices!);

            if (client == null)
            {
                return ServiceResult<Client>.Failure($"Client with id {clientId} not found");
            }

            return ServiceResult<Client>.Success(client);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client with all relations {ClientId}", clientId);
            return ServiceResult<Client>.Failure("An error occurred while retrieving the client");
        }
    }

    public async Task<ServiceResult<bool>> ArchiveClientAsync(int clientId)
    {
        try
        {
            var client = await GetEntityByIdAsync(clientId);
            if (client == null)
            {
                return ServiceResult<bool>.Failure($"Client with id {clientId} not found");
            }

            client.IsArchived = true;
            client.ArchivedAt = DateTime.UtcNow;
            await UpdateEntityAsync(client);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving client {ClientId}", clientId);
            return ServiceResult<bool>.Failure("An error occurred while archiving the client");
        }
    }

    public async Task<ServiceResult<bool>> UnarchiveClientAsync(int clientId)
    {
        try
        {
            var client = await GetEntityByIdAsync(clientId);
            if (client == null)
            {
                return ServiceResult<bool>.Failure($"Client with id {clientId} not found");
            }

            client.IsArchived = false;
            client.ArchivedAt = null;
            await UpdateEntityAsync(client);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unarchiving client {ClientId}", clientId);
            return ServiceResult<bool>.Failure("An error occurred while unarchiving the client");
        }
    }

    public async Task<ServiceResult<bool>> SetPriorityAsync(int clientId, string priority)
    {
        try
        {
            var client = await GetEntityByIdAsync(clientId);
            if (client == null)
            {
                return ServiceResult<bool>.Failure($"Client with id {clientId} not found");
            }

            client.Priority = priority;
            await UpdateEntityAsync(client);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting priority for client {ClientId}", clientId);
            return ServiceResult<bool>.Failure("An error occurred while setting the client priority");
        }
    }
    #endregion
} 