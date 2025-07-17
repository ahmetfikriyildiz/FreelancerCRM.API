using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerCRM.API.Models;

public class Assignment : BaseEntity
{
    [Key]
    public int AssignmentID { get; set; }
    public int TaskID => AssignmentID; // Mapping için alias
    [ForeignKey("Project")]
    public int ProjectID { get; set; }
    [ForeignKey("User")]
    public int UserID { get; set; }
    [Required, MaxLength(255)]
    public string AssignmentName { get; set; } = null!;
    public string TaskName => AssignmentName; // Mapping için alias;
    public string? Description { get; set; }
    [MaxLength(100)]
    public string? AssignedTo { get; set; }
    [MaxLength(20)]
    public string? Status { get; set; }
    [MaxLength(20)]
    public string? Priority { get; set; }
    public decimal EstimatedHours { get; set; }
    public decimal ActualHours { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime? CompletedAt => CompletedDate; // Mapping için alias
    public bool IsActive => Status != "Cancelled";
    // Navigation Properties
    public Project? Project { get; set; }
    public User? User { get; set; }
    public ICollection<TimeEntry>? TimeEntries { get; set; }
} 