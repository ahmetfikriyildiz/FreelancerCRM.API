using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerCRM.API.Models;

public class Project
{
    [Key]
    public int ProjectID { get; set; }
    [ForeignKey("User")]
    public int UserID { get; set; }
    [ForeignKey("Client")]
    public int ClientID { get; set; }
    [Required, MaxLength(255)]
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal EstimatedHours { get; set; }
    public decimal ActualHours { get; set; }
    [MaxLength(20)]
    public string? Status { get; set; }
    [MaxLength(20)]
    public string? Priority { get; set; }
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    [MaxLength(20)]
    public string? ContractType { get; set; }
    public decimal HourlyRate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    // Navigation Properties
    public User? User { get; set; }
    public Client? Client { get; set; }
    public ICollection<Assignment>? Tasks { get; set; }
    public ICollection<TimeEntry>? TimeEntries { get; set; }
    public ICollection<Invoice>? Invoices { get; set; }
} 