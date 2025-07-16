using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Repositories.Interfaces;

public interface IClientRepository : IGenericRepository<Client>
{
    Task<IEnumerable<Client>> GetClientsByUserIdAsync(int userId);
    Task<IEnumerable<Client>> GetActiveClientsAsync();
    Task<IEnumerable<Client>> GetClientsByStatusAsync(string status);
    Task<IEnumerable<Client>> GetClientsByPriorityAsync(string priority);
    Task<IEnumerable<Client>> GetClientsByIndustryAsync(string industry);
    Task<IEnumerable<Client>> GetClientsByCityAsync(string city);
    Task<Client?> GetClientWithProjectsAsync(int clientId);
    Task<Client?> GetClientWithInvoicesAsync(int clientId);
    Task<Client?> GetClientWithAllRelationsAsync(int clientId);
    Task<IEnumerable<Client>> SearchClientsAsync(string searchTerm);
} 