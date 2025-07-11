using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Services.Interfaces;

public interface ITimeEntryService : IBaseService<TimeEntry>
{
    Task<ServiceResult<IEnumerable<TimeEntry>>> GetTimeEntriesByUserIdAsync(int userId);
    Task<ServiceResult<IEnumerable<TimeEntry>>> GetTimeEntriesByProjectIdAsync(int projectId);
    Task<ServiceResult<IEnumerable<TimeEntry>>> GetTimeEntriesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<ServiceResult<TimeEntry>> StartTimeTrackingAsync(int userId, int projectId, int? assignmentId = null);
    Task<ServiceResult<TimeEntry>> StopTimeTrackingAsync(int userId);
    Task<ServiceResult<TimeEntry>> GetActiveTimeEntryAsync(int userId);
    Task<ServiceResult<bool>> UpdateTimeEntryAsync(int timeEntryId, string description, bool isBillable);
    Task<ServiceResult<decimal>> GetTotalHoursByUserAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<ServiceResult<decimal>> GetTotalHoursByProjectAsync(int projectId, DateTime? startDate = null, DateTime? endDate = null);
    Task<ServiceResult<decimal>> GetTotalBillableHoursAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<ServiceResult<decimal>> GetTotalEarningsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<ServiceResult<IEnumerable<TimeEntry>>> GetBillableTimeEntriesAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<ServiceResult<bool>> CalculateAmountAsync(int timeEntryId);
    Task<ServiceResult<bool>> BulkUpdateBillableStatusAsync(List<int> timeEntryIds, bool isBillable);
} 