using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Services.Interfaces;

public interface IProjectService : IBaseService<Project, ProjectCreateDto, ProjectUpdateDto, ProjectResponseDto, ProjectSummaryDto>
{
    Task<ServiceResult<IEnumerable<Project>>> GetProjectsByUserIdAsync(int userId);
    Task<ServiceResult<IEnumerable<Project>>> GetProjectsByClientIdAsync(int clientId);
    Task<ServiceResult<IEnumerable<Project>>> GetActiveProjectsAsync();
    Task<ServiceResult<IEnumerable<Project>>> GetCompletedProjectsAsync();
    Task<ServiceResult<IEnumerable<Project>>> GetOverdueProjectsAsync();
    Task<ServiceResult<Project>> GetProjectWithAllRelationsAsync(int projectId);
    Task<ServiceResult<bool>> StartProjectAsync(int projectId);
    Task<ServiceResult<bool>> CompleteProjectAsync(int projectId);
    Task<ServiceResult<bool>> PauseProjectAsync(int projectId);
    Task<ServiceResult<bool>> CancelProjectAsync(int projectId);
    Task<ServiceResult<decimal>> CalculateProjectProfitabilityAsync(int projectId);
    Task<ServiceResult<decimal>> GetProjectProgressAsync(int projectId);
    Task<ServiceResult<bool>> UpdateBudgetAsync(int projectId, decimal newBudget);
    Task<ServiceResult<bool>> ExtendDeadlineAsync(int projectId, DateTime newEndDate);
    Task<ServiceResult<IEnumerable<Project>>> GetProjectsByStatusAsync(string status);
    Task<ServiceResult<IEnumerable<Project>>> GetProjectsByDateRangeAsync(DateTime startDate, DateTime endDate);
} 