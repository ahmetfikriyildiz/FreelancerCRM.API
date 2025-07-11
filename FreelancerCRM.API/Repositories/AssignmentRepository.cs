using Microsoft.EntityFrameworkCore;
using FreelancerCRM.API.Data;
using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;

namespace FreelancerCRM.API.Repositories;

public class AssignmentRepository : GenericRepository<Assignment>, IAssignmentRepository
{
    public AssignmentRepository(FreelancerCrmDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Assignment>> GetAssignmentsByProjectIdAsync(int projectId)
    {
        return await _dbSet.Where(a => a.ProjectID == projectId).ToListAsync();
    }

    public async Task<IEnumerable<Assignment>> GetAssignmentsByStatusAsync(string status)
    {
        return await _dbSet.Where(a => a.Status == status).ToListAsync();
    }

    public async Task<IEnumerable<Assignment>> GetAssignmentsByPriorityAsync(string priority)
    {
        return await _dbSet.Where(a => a.Priority == priority).ToListAsync();
    }

    public async Task<IEnumerable<Assignment>> GetAssignmentsByAssignedToAsync(string assignedTo)
    {
        return await _dbSet.Where(a => a.AssignedTo == assignedTo).ToListAsync();
    }

    public async Task<IEnumerable<Assignment>> GetOverdueAssignmentsAsync()
    {
        return await _dbSet
            .Where(a => a.DueDate.HasValue && a.DueDate < DateTime.Now && a.Status != "Completed")
            .ToListAsync();
    }

    public async Task<IEnumerable<Assignment>> GetCompletedAssignmentsAsync()
    {
        return await _dbSet.Where(a => a.Status == "Completed").ToListAsync();
    }

    public async Task<IEnumerable<Assignment>> GetAssignmentsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(a => a.StartDate.HasValue && a.StartDate >= startDate && a.StartDate <= endDate)
            .ToListAsync();
    }

    public async Task<Assignment?> GetAssignmentWithTimeEntriesAsync(int assignmentId)
    {
        return await _dbSet
            .Include(a => a.TimeEntries)
            .FirstOrDefaultAsync(a => a.AssignmentID == assignmentId);
    }

    public async Task<decimal> GetTotalEstimatedHoursByProjectAsync(int projectId)
    {
        return await _dbSet
            .Where(a => a.ProjectID == projectId)
            .SumAsync(a => a.EstimatedHours);
    }

    public async Task<decimal> GetTotalActualHoursByProjectAsync(int projectId)
    {
        return await _dbSet
            .Where(a => a.ProjectID == projectId)
            .SumAsync(a => a.ActualHours);
    }
} 