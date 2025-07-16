using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerCRM.API.Models;

public class TimeEntry : BaseEntity
{
    [Key]
    public int TimeEntryID { get; set; }
    [ForeignKey("User")]
    public int UserID { get; set; }
    [ForeignKey("Project")]
    public int ProjectID { get; set; }
    [ForeignKey("Assignment")]
    public int? AssignmentID { get; set; }
    public int? TaskID => AssignmentID; // Mapping için alias
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Duration => DurationMinutes / 60.0m; // Mapping için saat cinsinden
    public bool IsBillable { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal Amount { get; set; }
    public decimal GrossAmount => Amount; // Mapping için alias
    public decimal WithholdingTaxRate => StopajRate * 100; // Yüzde olarak
    public decimal WithholdingTaxAmount => StopajAmount; // Mapping için alias
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    // Türkiye'ye özgü alanlar
    public decimal StopajRate { get; set; } = 0.20m;
    public decimal StopajAmount { get; set; }
    public decimal NetAmount { get; set; }
    // Navigation Properties
    public User? User { get; set; }
    public Project? Project { get; set; }
    public Assignment? Assignment { get; set; }
    public Assignment? Task => Assignment; // Mapping için alias
} 