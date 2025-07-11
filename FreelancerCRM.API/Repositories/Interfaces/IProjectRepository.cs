using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Repositories.Interfaces;

public interface IProjectRepository : IGenericRepository<Project>
{
    Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId);
    Task<IEnumerable<Project>> GetProjectsByClientIdAsync(int clientId);
    Task<IEnumerable<Project>> GetProjectsByStatusAsync(string status);
    Task<IEnumerable<Project>> GetProjectsByPriorityAsync(string priority);
    Task<IEnumerable<Project>> GetActiveProjectsAsync();
    Task<IEnumerable<Project>> GetCompletedProjectsAsync();
    Task<IEnumerable<Project>> GetOverdueProjectsAsync();
    Task<IEnumerable<Project>> GetProjectsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Project?> GetProjectWithAssignmentsAsync(int projectId);
    Task<Project?> GetProjectWithTimeEntriesAsync(int projectId);
    Task<Project?> GetProjectWithInvoicesAsync(int projectId);
    Task<Project?> GetProjectWithAllRelationsAsync(int projectId);
    Task<decimal> GetTotalBudgetByUserAsync(int userId);
    Task<decimal> GetTotalActualCostByUserAsync(int userId);
} 