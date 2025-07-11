using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FreelancerCRM.API.Services;

public class ProjectService : BaseService<Project>, IProjectService
{
    public ProjectService(IUnitOfWork unitOfWork, ILogger<ProjectService> logger) 
        : base(unitOfWork, logger)
    {
    }

    protected override async Task<Project?> GetEntityByIdAsync(int id)
    {
        return await _unitOfWork.Projects.GetByIdAsync(id);
    }

    protected override async Task<IEnumerable<Project>> GetAllEntitiesAsync()
    {
        return await _unitOfWork.Projects.GetAllAsync();
    }

    protected override async Task<Project> CreateEntityAsync(Project entity)
    {
        return await _unitOfWork.Projects.AddAsync(entity);
    }

    protected override async Task UpdateEntityAsync(Project entity)
    {
        await _unitOfWork.Projects.UpdateAsync(entity);
    }

    protected override async Task DeleteEntityAsync(Project entity)
    {
        await _unitOfWork.Projects.DeleteAsync(entity);
    }

    protected override async Task<bool> EntityExistsAsync(int id)
    {
        return await _unitOfWork.Projects.ExistsAsync(p => p.ProjectID == id);
    }

    protected override async Task<ValidationResult> ValidateEntityAsync(Project entity, bool isUpdate)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(entity.ProjectName))
        {
            errors.Add("Project name is required");
        }

        if (entity.UserID <= 0)
        {
            errors.Add("User ID is required");
        }

        if (entity.ClientID <= 0)
        {
            errors.Add("Client ID is required");
        }

        await Task.CompletedTask;
        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    protected override async Task<(bool CanDelete, string Reason)> CanDeleteEntityAsync(Project entity)
    {
        var timeEntryCount = await _unitOfWork.TimeEntries.CountAsync(t => t.ProjectID == entity.ProjectID);
        if (timeEntryCount > 0)
        {
            return (false, "Cannot delete project with existing time entries");
        }

        return (true, string.Empty);
    }

    // Implement interface methods with basic functionality
    public async Task<ServiceResult<IEnumerable<Project>>> GetProjectsByUserIdAsync(int userId)
    {
        try
        {
            var projects = await _unitOfWork.Projects.GetProjectsByUserIdAsync(userId);
            return ServiceResult<IEnumerable<Project>>.Success(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting projects for user {UserId}", userId);
            return ServiceResult<IEnumerable<Project>>.Failure("An error occurred while retrieving projects");
        }
    }

    public async Task<ServiceResult<IEnumerable<Project>>> GetProjectsByClientIdAsync(int clientId)
    {
        try
        {
            var projects = await _unitOfWork.Projects.GetProjectsByClientIdAsync(clientId);
            return ServiceResult<IEnumerable<Project>>.Success(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting projects for client {ClientId}", clientId);
            return ServiceResult<IEnumerable<Project>>.Failure("An error occurred while retrieving projects");
        }
    }

    public async Task<ServiceResult<IEnumerable<Project>>> GetActiveProjectsAsync()
    {
        try
        {
            var projects = await _unitOfWork.Projects.GetActiveProjectsAsync();
            return ServiceResult<IEnumerable<Project>>.Success(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active projects");
            return ServiceResult<IEnumerable<Project>>.Failure("An error occurred while retrieving active projects");
        }
    }

    public async Task<ServiceResult<IEnumerable<Project>>> GetCompletedProjectsAsync()
    {
        try
        {
            var projects = await _unitOfWork.Projects.GetCompletedProjectsAsync();
            return ServiceResult<IEnumerable<Project>>.Success(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting completed projects");
            return ServiceResult<IEnumerable<Project>>.Failure("An error occurred while retrieving completed projects");
        }
    }

    public async Task<ServiceResult<IEnumerable<Project>>> GetOverdueProjectsAsync()
    {
        try
        {
            var projects = await _unitOfWork.Projects.GetOverdueProjectsAsync();
            return ServiceResult<IEnumerable<Project>>.Success(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting overdue projects");
            return ServiceResult<IEnumerable<Project>>.Failure("An error occurred while retrieving overdue projects");
        }
    }

    public async Task<ServiceResult<Project>> GetProjectWithAllRelationsAsync(int projectId)
    {
        try
        {
            var project = await _unitOfWork.Projects.GetProjectWithAllRelationsAsync(projectId);
            if (project == null)
            {
                return ServiceResult<Project>.Failure("Project not found");
            }

            return ServiceResult<Project>.Success(project);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting project with relations {ProjectId}", projectId);
            return ServiceResult<Project>.Failure("An error occurred while retrieving project");
        }
    }

    // TODO: Implement remaining interface methods
    public Task<ServiceResult<bool>> StartProjectAsync(int projectId) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> CompleteProjectAsync(int projectId) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> PauseProjectAsync(int projectId) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> CancelProjectAsync(int projectId) => throw new NotImplementedException();
    public Task<ServiceResult<decimal>> CalculateProjectProfitabilityAsync(int projectId) => throw new NotImplementedException();
    public Task<ServiceResult<decimal>> GetProjectProgressAsync(int projectId) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> UpdateBudgetAsync(int projectId, decimal newBudget) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> ExtendDeadlineAsync(int projectId, DateTime newEndDate) => throw new NotImplementedException();
    public Task<ServiceResult<IEnumerable<Project>>> GetProjectsByStatusAsync(string status) => throw new NotImplementedException();
    public Task<ServiceResult<IEnumerable<Project>>> GetProjectsByDateRangeAsync(DateTime startDate, DateTime endDate) => throw new NotImplementedException();
} 