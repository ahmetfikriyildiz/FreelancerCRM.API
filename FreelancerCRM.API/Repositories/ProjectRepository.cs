using Microsoft.EntityFrameworkCore;
using FreelancerCRM.API.Data;
using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;

namespace FreelancerCRM.API.Repositories;

public class ProjectRepository : GenericRepository<Project>, IProjectRepository
{
    public ProjectRepository(FreelancerCrmDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId)
    {
        return await _dbSet.Where(p => p.UserID == userId).ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetProjectsByClientIdAsync(int clientId)
    {
        return await _dbSet.Where(p => p.ClientID == clientId).ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetProjectsByStatusAsync(string status)
    {
        return await _dbSet.Where(p => p.Status == status).ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetProjectsByPriorityAsync(string priority)
    {
        return await _dbSet.Where(p => p.Priority == priority).ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
    {
        return await _dbSet.Where(p => p.Status == "InProgress").ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetCompletedProjectsAsync()
    {
        return await _dbSet.Where(p => p.Status == "Completed").ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetOverdueProjectsAsync()
    {
        return await _dbSet
            .Where(p => p.EndDate.HasValue && p.EndDate < DateTime.Now && p.Status != "Completed")
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetProjectsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(p => p.StartDate >= startDate && p.StartDate <= endDate)
            .ToListAsync();
    }

    public async Task<Project?> GetProjectWithAssignmentsAsync(int projectId)
    {
        return await _dbSet
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.ProjectID == projectId);
    }

    public async Task<Project?> GetProjectWithTimeEntriesAsync(int projectId)
    {
        return await _dbSet
            .Include(p => p.TimeEntries)
            .FirstOrDefaultAsync(p => p.ProjectID == projectId);
    }

    public async Task<Project?> GetProjectWithInvoicesAsync(int projectId)
    {
        return await _dbSet
            .Include(p => p.Invoices)
            .FirstOrDefaultAsync(p => p.ProjectID == projectId);
    }

    public async Task<Project?> GetProjectWithAllRelationsAsync(int projectId)
    {
        return await _dbSet
            .Include(p => p.User)
            .Include(p => p.Client)
            .Include(p => p.Tasks)
            .Include(p => p.TimeEntries)
            .Include(p => p.Invoices)
            .FirstOrDefaultAsync(p => p.ProjectID == projectId);
    }

    public async Task<decimal> GetTotalBudgetByUserAsync(int userId)
    {
        return await _dbSet
            .Where(p => p.UserID == userId)
            .SumAsync(p => p.Budget);
    }

    public async Task<decimal> GetTotalActualCostByUserAsync(int userId)
    {
        return await _dbSet
            .Where(p => p.UserID == userId)
            .SumAsync(p => p.ActualCost);
    }
} 