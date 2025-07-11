using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Services.Interfaces;

public interface IClientService : IBaseService<Client>
{
    Task<ServiceResult<IEnumerable<Client>>> GetClientsByUserIdAsync(int userId);
    Task<ServiceResult<IEnumerable<Client>>> GetActiveClientsAsync();
    Task<ServiceResult<IEnumerable<Client>>> GetClientsByStatusAsync(string status);
    Task<ServiceResult<IEnumerable<Client>>> SearchClientsAsync(string searchTerm);
    Task<ServiceResult<Client>> GetClientWithProjectsAsync(int clientId);
    Task<ServiceResult<Client>> GetClientWithInvoicesAsync(int clientId);
    Task<ServiceResult<Client>> GetClientWithAllRelationsAsync(int clientId);
    Task<ServiceResult<bool>> ArchiveClientAsync(int clientId);
    Task<ServiceResult<bool>> UnarchiveClientAsync(int clientId);
    Task<ServiceResult<bool>> SetPriorityAsync(int clientId, string priority);
    Task<ServiceResult<bool>> ValidateTaxNumberAsync(string taxNumber);
    Task<ServiceResult<IEnumerable<Client>>> GetClientsByIndustryAsync(string industry);
    Task<ServiceResult<IEnumerable<Client>>> GetClientsByCityAsync(string city);
} 