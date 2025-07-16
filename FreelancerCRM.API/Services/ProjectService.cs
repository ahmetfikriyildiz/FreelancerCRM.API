using AutoMapper;
using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FreelancerCRM.API.Services;

public class ProjectService : BaseService<Project, ProjectCreateDto, ProjectUpdateDto, ProjectResponseDto, ProjectSummaryDto>, IProjectService
{
    public ProjectService(
        IUnitOfWork unitOfWork,
        ILogger<ProjectService> logger,
        IMapper mapper)
        : base(unitOfWork, logger, mapper)
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
        // Check if project has any tasks
        var hasTasks = await _unitOfWork.Assignments.ExistsAsync(a => a.ProjectID == entity.ProjectID);
        if (hasTasks)
        {
            return (false, "Cannot delete project with existing tasks");
        }

        // Check if project has any time entries
        var hasTimeEntries = await _unitOfWork.TimeEntries.ExistsAsync(t => t.ProjectID == entity.ProjectID);
        if (hasTimeEntries)
        {
            return (false, "Cannot delete project with existing time entries");
        }

        // Check if project has any invoices
        var hasInvoices = await _unitOfWork.Invoices.ExistsAsync(i => i.ProjectID == entity.ProjectID);
        if (hasInvoices)
        {
            return (false, "Cannot delete project with existing invoices");
        }

        return (true, string.Empty);
    }

    public async Task<ServiceResult<IEnumerable<Project>>> GetProjectsByUserIdAsync(int userId)
    {
        try
        {
            var projects = await _unitOfWork.Projects.FindAsync(p => p.UserID == userId);
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
            var projects = await _unitOfWork.Projects.FindAsync(p => p.ClientID == clientId);
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
            var projects = await _unitOfWork.Projects.FindAsync(p => p.IsActive && p.Status != "Completed" && p.Status != "Cancelled");
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
            var projects = await _unitOfWork.Projects.FindAsync(p => p.Status == "Completed");
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
            var now = DateTime.UtcNow;
            var projects = await _unitOfWork.Projects.FindAsync(
                p => p.EndDate.HasValue && 
                     p.EndDate.Value < now && 
                     p.Status != "Completed" && 
                     p.Status != "Cancelled");
            
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
            var project = await _unitOfWork.Projects.GetByIdWithIncludesAsync(
                projectId,
                p => p.Tasks!,
                p => p.TimeEntries!,
                p => p.Invoices!);

            if (project == null)
            {
                return ServiceResult<Project>.Failure($"Project with id {projectId} not found");
            }

            return ServiceResult<Project>.Success(project);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting project with relations {ProjectId}", projectId);
            return ServiceResult<Project>.Failure("An error occurred while retrieving project");
        }
    }

    public async Task<ServiceResult<bool>> StartProjectAsync(int projectId)
    {
        try
        {
            var project = await GetEntityByIdAsync(projectId);
            if (project == null)
            {
                return ServiceResult<bool>.Failure($"Project with id {projectId} not found");
            }

            project.Status = "InProgress";
            project.StartDate = DateTime.UtcNow;
            await UpdateEntityAsync(project);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting project {ProjectId}", projectId);
            return ServiceResult<bool>.Failure("An error occurred while starting the project");
        }
    }

    public async Task<ServiceResult<bool>> CompleteProjectAsync(int projectId)
    {
        try
        {
            var project = await GetEntityByIdAsync(projectId);
            if (project == null)
            {
                return ServiceResult<bool>.Failure($"Project with id {projectId} not found");
            }

            project.Status = "Completed";
            project.EndDate = DateTime.UtcNow;
            await UpdateEntityAsync(project);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing project {ProjectId}", projectId);
            return ServiceResult<bool>.Failure("An error occurred while completing the project");
        }
    }

    public async Task<ServiceResult<bool>> PauseProjectAsync(int projectId)
    {
        try
        {
            var project = await GetEntityByIdAsync(projectId);
            if (project == null)
            {
                return ServiceResult<bool>.Failure($"Project with id {projectId} not found");
            }

            project.Status = "OnHold";
            await UpdateEntityAsync(project);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pausing project {ProjectId}", projectId);
            return ServiceResult<bool>.Failure("An error occurred while pausing the project");
        }
    }

    public async Task<ServiceResult<bool>> CancelProjectAsync(int projectId)
    {
        try
        {
            var project = await GetEntityByIdAsync(projectId);
            if (project == null)
            {
                return ServiceResult<bool>.Failure($"Project with id {projectId} not found");
            }

            project.Status = "Cancelled";
            project.IsActive = false;
            await UpdateEntityAsync(project);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling project {ProjectId}", projectId);
            return ServiceResult<bool>.Failure("An error occurred while cancelling the project");
        }
    }

    public async Task<ServiceResult<decimal>> CalculateProjectProfitabilityAsync(int projectId)
    {
        try
        {
            var project = await GetEntityByIdAsync(projectId);
            if (project == null)
            {
                return ServiceResult<decimal>.Failure($"Project with id {projectId} not found");
            }

            // Get all time entries for the project
            var timeEntries = await _unitOfWork.TimeEntries.FindAsync(t => t.ProjectID == projectId && t.IsBillable);
            var totalRevenue = timeEntries.Sum(t => t.Amount);
            var profitability = (totalRevenue - project.ActualCost) / project.Budget * 100;

            return ServiceResult<decimal>.Success(profitability);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating profitability for project {ProjectId}", projectId);
            return ServiceResult<decimal>.Failure("An error occurred while calculating project profitability");
        }
    }

    public async Task<ServiceResult<decimal>> GetProjectProgressAsync(int projectId)
    {
        try
        {
            var project = await GetEntityByIdAsync(projectId);
            if (project == null)
            {
                return ServiceResult<decimal>.Failure($"Project with id {projectId} not found");
            }

            if (project.EstimatedHours <= 0)
            {
                return ServiceResult<decimal>.Success(0);
            }

            var progress = (project.ActualHours / project.EstimatedHours) * 100;
            return ServiceResult<decimal>.Success(Math.Min(progress, 100));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating progress for project {ProjectId}", projectId);
            return ServiceResult<decimal>.Failure("An error occurred while calculating project progress");
        }
    }

    public async Task<ServiceResult<bool>> UpdateBudgetAsync(int projectId, decimal newBudget)
    {
        try
        {
            var project = await GetEntityByIdAsync(projectId);
            if (project == null)
            {
                return ServiceResult<bool>.Failure($"Project with id {projectId} not found");
            }

            project.Budget = newBudget;
            await UpdateEntityAsync(project);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating budget for project {ProjectId}", projectId);
            return ServiceResult<bool>.Failure("An error occurred while updating the project budget");
        }
    }

    public async Task<ServiceResult<bool>> ExtendDeadlineAsync(int projectId, DateTime newEndDate)
    {
        try
        {
            var project = await GetEntityByIdAsync(projectId);
            if (project == null)
            {
                return ServiceResult<bool>.Failure($"Project with id {projectId} not found");
            }

            project.EndDate = newEndDate;
            await UpdateEntityAsync(project);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extending deadline for project {ProjectId}", projectId);
            return ServiceResult<bool>.Failure("An error occurred while extending the project deadline");
        }
    }

    public async Task<ServiceResult<IEnumerable<Project>>> GetProjectsByStatusAsync(string status)
    {
        try
        {
            var projects = await _unitOfWork.Projects.FindAsync(p => p.Status == status);
            return ServiceResult<IEnumerable<Project>>.Success(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting projects with status {Status}", status);
            return ServiceResult<IEnumerable<Project>>.Failure("An error occurred while retrieving projects");
        }
    }

    public async Task<ServiceResult<IEnumerable<Project>>> GetProjectsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var projects = await _unitOfWork.Projects.FindAsync(
                p => p.StartDate >= startDate && 
                     (!p.EndDate.HasValue || p.EndDate <= endDate));
            
            return ServiceResult<IEnumerable<Project>>.Success(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting projects between {StartDate} and {EndDate}", startDate, endDate);
            return ServiceResult<IEnumerable<Project>>.Failure("An error occurred while retrieving projects");
        }
    }
} 