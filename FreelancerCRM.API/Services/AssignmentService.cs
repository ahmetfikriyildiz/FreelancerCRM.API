using AutoMapper;
using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FreelancerCRM.API.Services;

public class AssignmentService : BaseService<Assignment, AssignmentCreateDto, AssignmentUpdateDto, AssignmentResponseDto, AssignmentSummaryDto>, IAssignmentService
{
    public AssignmentService(
        IUnitOfWork unitOfWork,
        ILogger<AssignmentService> logger,
        IMapper mapper)
        : base(unitOfWork, logger, mapper)
    {
    }

    protected override async Task<Assignment?> GetEntityByIdAsync(int id)
    {
        return await _unitOfWork.Assignments.GetByIdAsync(id);
    }

    protected override async Task<IEnumerable<Assignment>> GetAllEntitiesAsync()
    {
        return await _unitOfWork.Assignments.GetAllAsync();
    }

    protected override async Task<Assignment> CreateEntityAsync(Assignment entity)
    {
        return await _unitOfWork.Assignments.AddAsync(entity);
    }

    protected override async Task UpdateEntityAsync(Assignment entity)
    {
        await _unitOfWork.Assignments.UpdateAsync(entity);
    }

    protected override async Task DeleteEntityAsync(Assignment entity)
    {
        await _unitOfWork.Assignments.DeleteAsync(entity);
    }

    protected override async Task<ValidationResult> ValidateEntityAsync(Assignment entity, bool isUpdate)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(entity.AssignmentName))
        {
            errors.Add("Assignment name is required");
        }

        if (entity.ProjectID <= 0)
        {
            errors.Add("Project ID is required");
        }

        await Task.CompletedTask;
        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    protected override async Task<(bool CanDelete, string Reason)> CanDeleteEntityAsync(Assignment entity)
    {
        await Task.CompletedTask;
        return (true, string.Empty);
    }

    protected override async Task<bool> EntityExistsAsync(int id)
    {
        return await _unitOfWork.Assignments.ExistsAsync(a => a.AssignmentID == id);
    }

    // Basic implementations
    public async Task<ServiceResult<IEnumerable<Assignment>>> GetAssignmentsByProjectIdAsync(int projectId)
    {
        try
        {
            var assignments = await _unitOfWork.Assignments.FindAsync(a => a.ProjectID == projectId);
            return ServiceResult<IEnumerable<Assignment>>.Success(assignments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assignments for project {ProjectId}", projectId);
            return ServiceResult<IEnumerable<Assignment>>.Failure("An error occurred while retrieving assignments");
        }
    }

    public async Task<ServiceResult<IEnumerable<Assignment>>> GetAssignmentsByStatusAsync(string status)
    {
        try
        {
            var assignments = await _unitOfWork.Assignments.FindAsync(a => a.Status == status);
            return ServiceResult<IEnumerable<Assignment>>.Success(assignments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assignments with status {Status}", status);
            return ServiceResult<IEnumerable<Assignment>>.Failure("An error occurred while retrieving assignments");
        }
    }

    public async Task<ServiceResult<IEnumerable<Assignment>>> GetOverdueAssignmentsAsync()
    {
        try
        {
            var now = DateTime.UtcNow;
            var assignments = await _unitOfWork.Assignments.FindAsync(
                a => a.DueDate.HasValue && 
                     a.DueDate.Value < now && 
                     a.Status != "Completed" && 
                     a.Status != "Cancelled");
            
            return ServiceResult<IEnumerable<Assignment>>.Success(assignments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting overdue assignments");
            return ServiceResult<IEnumerable<Assignment>>.Failure("An error occurred while retrieving overdue assignments");
        }
    }

    public async Task<ServiceResult<IEnumerable<Assignment>>> GetAssignmentsByAssignedToAsync(string assignedTo)
    {
        try
        {
            var assignments = await _unitOfWork.Assignments.FindAsync(a => a.AssignedTo == assignedTo);
            return ServiceResult<IEnumerable<Assignment>>.Success(assignments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assignments for assignee {AssignedTo}", assignedTo);
            return ServiceResult<IEnumerable<Assignment>>.Failure("An error occurred while retrieving assignments");
        }
    }

    public async Task<ServiceResult<Assignment>> GetAssignmentWithTimeEntriesAsync(int assignmentId)
    {
        try
        {
            var assignment = await _unitOfWork.Assignments.GetByIdWithIncludesAsync(
                assignmentId,
                a => a.TimeEntries!);

            if (assignment == null)
            {
                return ServiceResult<Assignment>.Failure($"Assignment with id {assignmentId} not found");
            }

            return ServiceResult<Assignment>.Success(assignment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assignment with time entries {AssignmentId}", assignmentId);
            return ServiceResult<Assignment>.Failure("An error occurred while retrieving the assignment");
        }
    }

    public async Task<ServiceResult<bool>> StartAssignmentAsync(int assignmentId)
    {
        try
        {
            var assignment = await GetEntityByIdAsync(assignmentId);
            if (assignment == null)
            {
                return ServiceResult<bool>.Failure($"Assignment with id {assignmentId} not found");
            }

            assignment.Status = "InProgress";
            assignment.StartDate = DateTime.UtcNow;
            await UpdateEntityAsync(assignment);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting assignment {AssignmentId}", assignmentId);
            return ServiceResult<bool>.Failure("An error occurred while starting the assignment");
        }
    }

    public async Task<ServiceResult<bool>> CompleteAssignmentAsync(int assignmentId)
    {
        try
        {
            var assignment = await GetEntityByIdAsync(assignmentId);
            if (assignment == null)
            {
                return ServiceResult<bool>.Failure($"Assignment with id {assignmentId} not found");
            }

            assignment.Status = "Completed";
            assignment.CompletedDate = DateTime.UtcNow;
            await UpdateEntityAsync(assignment);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing assignment {AssignmentId}", assignmentId);
            return ServiceResult<bool>.Failure("An error occurred while completing the assignment");
        }
    }

    public async Task<ServiceResult<bool>> AssignToUserAsync(int assignmentId, string assignedTo)
    {
        try
        {
            var assignment = await GetEntityByIdAsync(assignmentId);
            if (assignment == null)
            {
                return ServiceResult<bool>.Failure($"Assignment with id {assignmentId} not found");
            }

            assignment.AssignedTo = assignedTo;
            await UpdateEntityAsync(assignment);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning assignment {AssignmentId} to {AssignedTo}", assignmentId, assignedTo);
            return ServiceResult<bool>.Failure("An error occurred while assigning the task");
        }
    }

    public async Task<ServiceResult<bool>> UpdateProgressAsync(int assignmentId, decimal actualHours)
    {
        try
        {
            var assignment = await GetEntityByIdAsync(assignmentId);
            if (assignment == null)
            {
                return ServiceResult<bool>.Failure($"Assignment with id {assignmentId} not found");
            }

            assignment.ActualHours = actualHours;
            await UpdateEntityAsync(assignment);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating progress for assignment {AssignmentId}", assignmentId);
            return ServiceResult<bool>.Failure("An error occurred while updating the progress");
        }
    }

    public async Task<ServiceResult<bool>> ExtendDeadlineAsync(int assignmentId, DateTime newDueDate)
    {
        try
        {
            var assignment = await GetEntityByIdAsync(assignmentId);
            if (assignment == null)
            {
                return ServiceResult<bool>.Failure($"Assignment with id {assignmentId} not found");
            }

            assignment.DueDate = newDueDate;
            await UpdateEntityAsync(assignment);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extending deadline for assignment {AssignmentId}", assignmentId);
            return ServiceResult<bool>.Failure("An error occurred while extending the deadline");
        }
    }

    public async Task<ServiceResult<decimal>> GetAssignmentProgressAsync(int assignmentId)
    {
        try
        {
            var assignment = await GetEntityByIdAsync(assignmentId);
            if (assignment == null)
            {
                return ServiceResult<decimal>.Failure($"Assignment with id {assignmentId} not found");
            }

            if (assignment.EstimatedHours <= 0)
            {
                return ServiceResult<decimal>.Success(0);
            }

            var progress = (assignment.ActualHours / assignment.EstimatedHours) * 100;
            return ServiceResult<decimal>.Success(Math.Min(progress, 100));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating progress for assignment {AssignmentId}", assignmentId);
            return ServiceResult<decimal>.Failure("An error occurred while calculating the progress");
        }
    }

    public async Task<ServiceResult<IEnumerable<Assignment>>> GetAssignmentsByPriorityAsync(string priority)
    {
        try
        {
            var assignments = await _unitOfWork.Assignments.FindAsync(a => a.Priority == priority);
            return ServiceResult<IEnumerable<Assignment>>.Success(assignments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assignments with priority {Priority}", priority);
            return ServiceResult<IEnumerable<Assignment>>.Failure("An error occurred while retrieving assignments");
        }
    }

    public async Task<ServiceResult<bool>> SetPriorityAsync(int assignmentId, string priority)
    {
        try
        {
            var assignment = await GetEntityByIdAsync(assignmentId);
            if (assignment == null)
            {
                return ServiceResult<bool>.Failure($"Assignment with id {assignmentId} not found");
            }

            assignment.Priority = priority;
            await UpdateEntityAsync(assignment);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting priority for assignment {AssignmentId}", assignmentId);
            return ServiceResult<bool>.Failure("An error occurred while setting the priority");
        }
    }
} 