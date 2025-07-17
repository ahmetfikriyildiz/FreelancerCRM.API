using AutoMapper;
using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FreelancerCRM.API.Services;

public class TimeEntryService : BaseService<TimeEntry, TimeEntryCreateDto, TimeEntryUpdateDto, TimeEntryResponseDto, TimeEntrySummaryDto>, ITimeEntryService
{
    public TimeEntryService(
        IUnitOfWork unitOfWork, 
        ILogger<TimeEntryService> logger,
        IMapper mapper) 
        : base(unitOfWork, logger, mapper)
    {
    }

    protected override async Task<TimeEntry?> GetEntityByIdAsync(int id)
    {
        return await _unitOfWork.TimeEntries.GetByIdAsync(id);
    }

    protected override async Task<IEnumerable<TimeEntry>> GetAllEntitiesAsync()
    {
        return await _unitOfWork.TimeEntries.GetAllAsync();
    }

    protected override async Task<TimeEntry> CreateEntityAsync(TimeEntry entity)
    {
        return await _unitOfWork.TimeEntries.AddAsync(entity);
    }

    protected override async Task UpdateEntityAsync(TimeEntry entity)
    {
        await _unitOfWork.TimeEntries.UpdateAsync(entity);
    }

    protected override async Task DeleteEntityAsync(TimeEntry entity)
    {
        await _unitOfWork.TimeEntries.DeleteAsync(entity);
    }

    protected override async Task<bool> EntityExistsAsync(int id)
    {
        return await _unitOfWork.TimeEntries.ExistsAsync(t => t.TimeEntryID == id);
    }

    protected override async Task<ValidationResult> ValidateEntityAsync(TimeEntry entity, bool isUpdate)
    {
        var errors = new List<string>();

        if (entity.UserID <= 0)
        {
            errors.Add("User ID is required");
        }

        if (entity.ProjectID <= 0)
        {
            errors.Add("Project ID is required");
        }

        await Task.CompletedTask;
        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    protected override async Task<(bool CanDelete, string Reason)> CanDeleteEntityAsync(TimeEntry entity)
    {
        await Task.CompletedTask;
        return (true, string.Empty);
    }

    // Basic implementations
    public async Task<ServiceResult<IEnumerable<TimeEntry>>> GetTimeEntriesByUserIdAsync(int userId)
    {
        try
        {
            var timeEntries = await _unitOfWork.TimeEntries.GetTimeEntriesByUserIdAsync(userId);
            return ServiceResult<IEnumerable<TimeEntry>>.Success(timeEntries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting time entries for user {UserId}", userId);
            return ServiceResult<IEnumerable<TimeEntry>>.Failure("An error occurred while retrieving time entries");
        }
    }

    public async Task<ServiceResult<IEnumerable<TimeEntry>>> GetTimeEntriesByProjectIdAsync(int projectId)
    {
        try
        {
            var timeEntries = await _unitOfWork.TimeEntries.GetTimeEntriesByProjectIdAsync(projectId);
            return ServiceResult<IEnumerable<TimeEntry>>.Success(timeEntries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting time entries for project {ProjectId}", projectId);
            return ServiceResult<IEnumerable<TimeEntry>>.Failure("An error occurred while retrieving time entries");
        }
    }

    public async Task<ServiceResult<TimeEntry>> StartTimeTrackingAsync(int userId, int projectId, int? assignmentId = null)
    {
        try
        {
            // Check if user has an active time entry
            var activeEntry = await _unitOfWork.TimeEntries.GetActiveTimeEntryByUserAsync(userId);
            if (activeEntry != null)
            {
                return ServiceResult<TimeEntry>.Failure("User already has an active time entry. Please stop the current one first.");
            }

            // Validate project exists
            var projectExists = await _unitOfWork.Projects.ExistsAsync(p => p.ProjectID == projectId);
            if (!projectExists)
            {
                return ServiceResult<TimeEntry>.Failure("Project not found");
            }

            // Validate assignment if provided
            if (assignmentId.HasValue)
            {
                var assignmentExists = await _unitOfWork.Assignments.ExistsAsync(a => a.AssignmentID == assignmentId.Value);
                if (!assignmentExists)
                {
                    return ServiceResult<TimeEntry>.Failure("Assignment not found");
                }
            }

            var timeEntry = new TimeEntry
            {
                UserID = userId,
                ProjectID = projectId,
                AssignmentID = assignmentId,
                StartTime = DateTime.UtcNow,
                Date = DateTime.UtcNow.Date,
                IsBillable = true, // Default to billable
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.BeginTransactionAsync();
            var createdEntry = await _unitOfWork.TimeEntries.AddAsync(timeEntry);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<TimeEntry>.Success(createdEntry);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error starting time tracking for user {UserId}", userId);
            return ServiceResult<TimeEntry>.Failure("An error occurred while starting time tracking");
        }
    }

    public async Task<ServiceResult<TimeEntry>> StopTimeTrackingAsync(int userId)
    {
        try
        {
            var activeEntry = await _unitOfWork.TimeEntries.GetActiveTimeEntryByUserAsync(userId);
            if (activeEntry == null)
            {
                return ServiceResult<TimeEntry>.Failure("No active time entry found for this user");
            }

            var endTime = DateTime.UtcNow;
            var duration = (int)(endTime - activeEntry.StartTime).TotalMinutes;

            activeEntry.EndTime = endTime;
            activeEntry.DurationMinutes = duration;
            activeEntry.UpdatedAt = DateTime.UtcNow;

            // Calculate amount based on hourly rate
            if (activeEntry.HourlyRate > 0)
            {
                var hours = duration / 60.0m;
                activeEntry.Amount = hours * activeEntry.HourlyRate;
                activeEntry.NetAmount = activeEntry.Amount;
            }

            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.TimeEntries.UpdateAsync(activeEntry);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<TimeEntry>.Success(activeEntry);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error stopping time tracking for user {UserId}", userId);
            return ServiceResult<TimeEntry>.Failure("An error occurred while stopping time tracking");
        }
    }

    public async Task<ServiceResult<TimeEntry>> GetActiveTimeEntryAsync(int userId)
    {
        try
        {
            var activeEntry = await _unitOfWork.TimeEntries.GetActiveTimeEntryByUserAsync(userId);
            if (activeEntry == null)
            {
                return ServiceResult<TimeEntry>.Failure("No active time entry found for this user");
            }

            return ServiceResult<TimeEntry>.Success(activeEntry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active time entry for user {UserId}", userId);
            return ServiceResult<TimeEntry>.Failure("An error occurred while retrieving active time entry");
        }
    }

    public async Task<ServiceResult<IEnumerable<TimeEntry>>> GetTimeEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var timeEntries = await _unitOfWork.TimeEntries.GetTimeEntriesByDateRangeAsync(startDate, endDate);
            return ServiceResult<IEnumerable<TimeEntry>>.Success(timeEntries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting time entries by date range");
            return ServiceResult<IEnumerable<TimeEntry>>.Failure("An error occurred while retrieving time entries");
        }
    }

    public async Task<ServiceResult<decimal>> GetTotalHoursByUserAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            IEnumerable<TimeEntry> timeEntries;
            
            if (startDate.HasValue && endDate.HasValue)
            {
                timeEntries = await _unitOfWork.TimeEntries.GetTimeEntriesByUserAndDateRangeAsync(userId, startDate.Value, endDate.Value);
            }
            else
            {
                timeEntries = await _unitOfWork.TimeEntries.GetTimeEntriesByUserIdAsync(userId);
            }

            var totalHours = timeEntries.Sum(t => t.DurationMinutes / 60.0m);
            return ServiceResult<decimal>.Success(totalHours);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total hours for user {UserId}", userId);
            return ServiceResult<decimal>.Failure("An error occurred while calculating total hours");
        }
    }

    public async Task<ServiceResult<decimal>> GetTotalBillableHoursAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            IEnumerable<TimeEntry> timeEntries;
            
            if (startDate.HasValue && endDate.HasValue)
            {
                timeEntries = await _unitOfWork.TimeEntries.GetTimeEntriesByUserAndDateRangeAsync(userId, startDate.Value, endDate.Value);
            }
            else
            {
                timeEntries = await _unitOfWork.TimeEntries.GetTimeEntriesByUserIdAsync(userId);
            }

            var totalBillableHours = timeEntries.Where(t => t.IsBillable).Sum(t => t.DurationMinutes / 60.0m);
            return ServiceResult<decimal>.Success(totalBillableHours);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total billable hours for user {UserId}", userId);
            return ServiceResult<decimal>.Failure("An error occurred while calculating total billable hours");
        }
    }

    // TODO: Implement remaining interface methods
    public async Task<ServiceResult<decimal>> GetTotalHoursByProjectAsync(int projectId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            IEnumerable<TimeEntry> timeEntries;
            
            if (startDate.HasValue && endDate.HasValue)
            {
                timeEntries = await _unitOfWork.TimeEntries.GetTimeEntriesByProjectAndDateRangeAsync(projectId, startDate.Value, endDate.Value);
            }
            else
            {
                timeEntries = await _unitOfWork.TimeEntries.GetTimeEntriesByProjectIdAsync(projectId);
            }

            var totalMinutes = timeEntries.Sum(te => te.DurationMinutes);
            var totalHours = totalMinutes / 60.0m;

            return ServiceResult<decimal>.Success(totalHours);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating total hours for project {ProjectId}", projectId);
            return ServiceResult<decimal>.Failure("An error occurred while calculating total hours");
        }
    }

    public async Task<ServiceResult<decimal>> GetTotalEarningsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            IEnumerable<TimeEntry> timeEntries;
            
            if (startDate.HasValue && endDate.HasValue)
            {
                timeEntries = await _unitOfWork.TimeEntries.GetTimeEntriesByUserAndDateRangeAsync(userId, startDate.Value, endDate.Value);
            }
            else
            {
                timeEntries = await _unitOfWork.TimeEntries.GetTimeEntriesByUserIdAsync(userId);
            }

            var totalEarnings = timeEntries.Where(te => te.IsBillable).Sum(te => te.Amount);
            return ServiceResult<decimal>.Success(totalEarnings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating total earnings for user {UserId}", userId);
            return ServiceResult<decimal>.Failure("An error occurred while calculating total earnings");
        }
    }

    public async Task<ServiceResult<IEnumerable<TimeEntry>>> GetBillableTimeEntriesAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            IEnumerable<TimeEntry> timeEntries;
            
            if (startDate.HasValue && endDate.HasValue)
            {
                timeEntries = await _unitOfWork.TimeEntries.GetTimeEntriesByUserAndDateRangeAsync(userId, startDate.Value, endDate.Value);
            }
            else
            {
                timeEntries = await _unitOfWork.TimeEntries.GetTimeEntriesByUserIdAsync(userId);
            }

            var billableEntries = timeEntries.Where(te => te.IsBillable).ToList();
            return ServiceResult<IEnumerable<TimeEntry>>.Success(billableEntries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting billable time entries for user {UserId}", userId);
            return ServiceResult<IEnumerable<TimeEntry>>.Failure("An error occurred while retrieving billable time entries");
        }
    }

    public async Task<ServiceResult<bool>> CalculateAmountAsync(int timeEntryId)
    {
        try
        {
            var timeEntry = await _unitOfWork.TimeEntries.GetByIdAsync(timeEntryId);
            if (timeEntry == null)
            {
                return ServiceResult<bool>.Failure("Time entry not found");
            }

            if (!timeEntry.EndTime.HasValue)
            {
                return ServiceResult<bool>.Failure("Cannot calculate amount for active time entry");
            }

            if (timeEntry.HourlyRate <= 0)
            {
                return ServiceResult<bool>.Failure("Hourly rate must be greater than zero");
            }

            await _unitOfWork.BeginTransactionAsync();

            var hours = timeEntry.DurationMinutes / 60.0m;
            timeEntry.Amount = hours * timeEntry.HourlyRate;
            timeEntry.NetAmount = timeEntry.Amount;
            timeEntry.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.TimeEntries.UpdateAsync(timeEntry);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error calculating amount for time entry {TimeEntryId}", timeEntryId);
            return ServiceResult<bool>.Failure("An error occurred while calculating amount");
        }
    }

    public async Task<ServiceResult<bool>> UpdateTimeEntryAsync(int timeEntryId, string description, bool isBillable)
    {
        try
        {
            var timeEntry = await _unitOfWork.TimeEntries.GetByIdAsync(timeEntryId);
            if (timeEntry == null)
            {
                return ServiceResult<bool>.Failure("Time entry not found");
            }

            await _unitOfWork.BeginTransactionAsync();

            timeEntry.Description = description;
            timeEntry.IsBillable = isBillable;
            timeEntry.UpdatedAt = DateTime.UtcNow;

            // Recalculate amount if billable status changed
            if (timeEntry.EndTime.HasValue && timeEntry.HourlyRate > 0)
            {
                var hours = timeEntry.DurationMinutes / 60.0m;
                timeEntry.Amount = isBillable ? hours * timeEntry.HourlyRate : 0;
                timeEntry.NetAmount = timeEntry.Amount;
            }

            await _unitOfWork.TimeEntries.UpdateAsync(timeEntry);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating time entry {TimeEntryId}", timeEntryId);
            return ServiceResult<bool>.Failure("An error occurred while updating time entry");
        }
    }

    public async Task<ServiceResult<bool>> BulkUpdateBillableStatusAsync(List<int> timeEntryIds, bool isBillable)
    {
        try
        {
            if (timeEntryIds == null || !timeEntryIds.Any())
            {
                return ServiceResult<bool>.Failure("No time entries provided");
            }

            await _unitOfWork.BeginTransactionAsync();

            foreach (var id in timeEntryIds)
            {
                var timeEntry = await _unitOfWork.TimeEntries.GetByIdAsync(id);
                if (timeEntry != null)
                {
                    timeEntry.IsBillable = isBillable;
                    timeEntry.UpdatedAt = DateTime.UtcNow;

                    // Recalculate amount if billable status changed
                    if (timeEntry.EndTime.HasValue && timeEntry.HourlyRate > 0)
                    {
                        var hours = timeEntry.DurationMinutes / 60.0m;
                        timeEntry.Amount = isBillable ? hours * timeEntry.HourlyRate : 0;
                        timeEntry.NetAmount = timeEntry.Amount;
                    }

                    await _unitOfWork.TimeEntries.UpdateAsync(timeEntry);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error bulk updating billable status for time entries");
            return ServiceResult<bool>.Failure("An error occurred while updating time entries");
        }
    }
} 