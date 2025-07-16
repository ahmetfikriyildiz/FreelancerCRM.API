using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Services.Interfaces;

public interface IAssignmentService : IBaseService<Assignment, AssignmentCreateDto, AssignmentUpdateDto, AssignmentResponseDto, AssignmentSummaryDto>
{
    Task<ServiceResult<IEnumerable<Assignment>>> GetAssignmentsByProjectIdAsync(int projectId);
    Task<ServiceResult<IEnumerable<Assignment>>> GetAssignmentsByStatusAsync(string status);
    Task<ServiceResult<IEnumerable<Assignment>>> GetOverdueAssignmentsAsync();
    Task<ServiceResult<IEnumerable<Assignment>>> GetAssignmentsByAssignedToAsync(string assignedTo);
    Task<ServiceResult<Assignment>> GetAssignmentWithTimeEntriesAsync(int assignmentId);
    Task<ServiceResult<bool>> StartAssignmentAsync(int assignmentId);
    Task<ServiceResult<bool>> CompleteAssignmentAsync(int assignmentId);
    Task<ServiceResult<bool>> AssignToUserAsync(int assignmentId, string assignedTo);
    Task<ServiceResult<bool>> UpdateProgressAsync(int assignmentId, decimal actualHours);
    Task<ServiceResult<bool>> ExtendDeadlineAsync(int assignmentId, DateTime newDueDate);
    Task<ServiceResult<decimal>> GetAssignmentProgressAsync(int assignmentId);
    Task<ServiceResult<IEnumerable<Assignment>>> GetAssignmentsByPriorityAsync(string priority);
    Task<ServiceResult<bool>> SetPriorityAsync(int assignmentId, string priority);
} 