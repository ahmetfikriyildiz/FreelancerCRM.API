using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Repositories.Interfaces;

public interface IAssignmentRepository : IGenericRepository<Assignment>
{
    Task<IEnumerable<Assignment>> GetAssignmentsByProjectIdAsync(int projectId);
    Task<IEnumerable<Assignment>> GetAssignmentsByStatusAsync(string status);
    Task<IEnumerable<Assignment>> GetAssignmentsByPriorityAsync(string priority);
    Task<IEnumerable<Assignment>> GetAssignmentsByAssignedToAsync(string assignedTo);
    Task<IEnumerable<Assignment>> GetOverdueAssignmentsAsync();
    Task<IEnumerable<Assignment>> GetCompletedAssignmentsAsync();
    Task<IEnumerable<Assignment>> GetAssignmentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Assignment?> GetAssignmentWithTimeEntriesAsync(int assignmentId);
    Task<decimal> GetTotalEstimatedHoursByProjectAsync(int projectId);
    Task<decimal> GetTotalActualHoursByProjectAsync(int projectId);
} 