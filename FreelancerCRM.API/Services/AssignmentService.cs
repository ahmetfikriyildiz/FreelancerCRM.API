using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FreelancerCRM.API.Services;

public class AssignmentService : BaseService<Assignment>, IAssignmentService
{
    public AssignmentService(IUnitOfWork unitOfWork, ILogger<AssignmentService> logger) 
        : base(unitOfWork, logger)
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

    protected override async Task<bool> EntityExistsAsync(int id)
    {
        return await _unitOfWork.Assignments.ExistsAsync(a => a.AssignmentID == id);
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

    // Basic implementations
    public async Task<ServiceResult<IEnumerable<Assignment>>> GetAssignmentsByProjectIdAsync(int projectId)
    {
        try
        {
            var assignments = await _unitOfWork.Assignments.GetAssignmentsByProjectIdAsync(projectId);
            return ServiceResult<IEnumerable<Assignment>>.Success(assignments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assignments for project {ProjectId}", projectId);
            return ServiceResult<IEnumerable<Assignment>>.Failure("An error occurred while retrieving assignments");
        }
    }

    // TODO: Implement remaining interface methods
    public Task<ServiceResult<IEnumerable<Assignment>>> GetAssignmentsByStatusAsync(string status) => throw new NotImplementedException();
    public Task<ServiceResult<IEnumerable<Assignment>>> GetOverdueAssignmentsAsync() => throw new NotImplementedException();
    public Task<ServiceResult<IEnumerable<Assignment>>> GetAssignmentsByAssignedToAsync(string assignedTo) => throw new NotImplementedException();
    public Task<ServiceResult<Assignment>> GetAssignmentWithTimeEntriesAsync(int assignmentId) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> StartAssignmentAsync(int assignmentId) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> CompleteAssignmentAsync(int assignmentId) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> AssignToUserAsync(int assignmentId, string assignedTo) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> UpdateProgressAsync(int assignmentId, decimal actualHours) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> ExtendDeadlineAsync(int assignmentId, DateTime newDueDate) => throw new NotImplementedException();
    public Task<ServiceResult<decimal>> GetAssignmentProgressAsync(int assignmentId) => throw new NotImplementedException();
    public Task<ServiceResult<IEnumerable<Assignment>>> GetAssignmentsByPriorityAsync(string priority) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> SetPriorityAsync(int assignmentId, string priority) => throw new NotImplementedException();
} 