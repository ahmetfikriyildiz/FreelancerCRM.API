using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Repositories.Interfaces;

public interface ITimeEntryRepository : IGenericRepository<TimeEntry>
{
    Task<IEnumerable<TimeEntry>> GetTimeEntriesByUserIdAsync(int userId);
    Task<IEnumerable<TimeEntry>> GetTimeEntriesByProjectIdAsync(int projectId);
    Task<IEnumerable<TimeEntry>> GetTimeEntriesByAssignmentIdAsync(int assignmentId);
    Task<IEnumerable<TimeEntry>> GetTimeEntriesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<TimeEntry>> GetBillableTimeEntriesAsync();
    Task<IEnumerable<TimeEntry>> GetNonBillableTimeEntriesAsync();
    Task<IEnumerable<TimeEntry>> GetTimeEntriesByUserAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<TimeEntry>> GetTimeEntriesByProjectAndDateRangeAsync(int projectId, DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalHoursByUserAsync(int userId);
    Task<decimal> GetTotalHoursByProjectAsync(int projectId);
    Task<decimal> GetTotalBillableHoursByUserAsync(int userId);
    Task<decimal> GetTotalAmountByUserAsync(int userId);
    Task<decimal> GetTotalAmountByProjectAsync(int projectId);
    Task<TimeEntry?> GetActiveTimeEntryByUserAsync(int userId);
} 