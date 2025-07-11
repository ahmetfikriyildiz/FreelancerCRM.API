using Microsoft.EntityFrameworkCore;
using FreelancerCRM.API.Data;
using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;

namespace FreelancerCRM.API.Repositories;

public class TimeEntryRepository : GenericRepository<TimeEntry>, ITimeEntryRepository
{
    public TimeEntryRepository(FreelancerCrmDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TimeEntry>> GetTimeEntriesByUserIdAsync(int userId)
    {
        return await _dbSet.Where(t => t.UserID == userId).ToListAsync();
    }

    public async Task<IEnumerable<TimeEntry>> GetTimeEntriesByProjectIdAsync(int projectId)
    {
        return await _dbSet.Where(t => t.ProjectID == projectId).ToListAsync();
    }

    public async Task<IEnumerable<TimeEntry>> GetTimeEntriesByAssignmentIdAsync(int assignmentId)
    {
        return await _dbSet.Where(t => t.AssignmentID == assignmentId).ToListAsync();
    }

    public async Task<IEnumerable<TimeEntry>> GetTimeEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<TimeEntry>> GetBillableTimeEntriesAsync()
    {
        return await _dbSet.Where(t => t.IsBillable).ToListAsync();
    }

    public async Task<IEnumerable<TimeEntry>> GetNonBillableTimeEntriesAsync()
    {
        return await _dbSet.Where(t => !t.IsBillable).ToListAsync();
    }

    public async Task<IEnumerable<TimeEntry>> GetTimeEntriesByUserAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(t => t.UserID == userId && t.Date >= startDate && t.Date <= endDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<TimeEntry>> GetTimeEntriesByProjectAndDateRangeAsync(int projectId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(t => t.ProjectID == projectId && t.Date >= startDate && t.Date <= endDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalHoursByUserAsync(int userId)
    {
        return await _dbSet
            .Where(t => t.UserID == userId)
            .SumAsync(t => t.DurationMinutes / 60.0m);
    }

    public async Task<decimal> GetTotalHoursByProjectAsync(int projectId)
    {
        return await _dbSet
            .Where(t => t.ProjectID == projectId)
            .SumAsync(t => t.DurationMinutes / 60.0m);
    }

    public async Task<decimal> GetTotalBillableHoursByUserAsync(int userId)
    {
        return await _dbSet
            .Where(t => t.UserID == userId && t.IsBillable)
            .SumAsync(t => t.DurationMinutes / 60.0m);
    }

    public async Task<decimal> GetTotalAmountByUserAsync(int userId)
    {
        return await _dbSet
            .Where(t => t.UserID == userId)
            .SumAsync(t => t.Amount);
    }

    public async Task<decimal> GetTotalAmountByProjectAsync(int projectId)
    {
        return await _dbSet
            .Where(t => t.ProjectID == projectId)
            .SumAsync(t => t.Amount);
    }

    public async Task<TimeEntry?> GetActiveTimeEntryByUserAsync(int userId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.UserID == userId && t.EndTime == null);
    }
} 