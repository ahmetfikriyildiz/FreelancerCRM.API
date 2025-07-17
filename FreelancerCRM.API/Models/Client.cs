using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerCRM.API.Models;

public class Client : BaseEntity
{
    [Key]
    public int ClientID { get; set; }
    [ForeignKey("User")]
    public int UserID { get; set; }
    [Required, MaxLength(255)]
    public string CompanyName { get; set; } = null!;
    [MaxLength(100)]
    public string? ContactFirstName { get; set; }
    [MaxLength(100)]
    public string? ContactLastName { get; set; }
    public string ContactName => $"{ContactFirstName} {ContactLastName}".Trim();
    [MaxLength(255)]
    public string? Email { get; set; }
    [MaxLength(20)]
    public string? Phone { get; set; }
    public string? PhoneNumber => Phone;
    [MaxLength(255)]
    public string? Address { get; set; }
    [MaxLength(100)]
    public string? City { get; set; }
    [MaxLength(100)]
    public string Country { get; set; } = "United States";
    [MaxLength(255)]
    public string? Website { get; set; }
    [MaxLength(100)]
    public string? Industry { get; set; }
    [MaxLength(50)]
    public string? CompanySize { get; set; }
    [MaxLength(20)]
    public string? Priority { get; set; }
    public bool IsActive => Status != "Inactive";
    [MaxLength(20)]
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public bool IsArchived { get; set; } = false;
    public DateTime? ArchivedAt { get; set; }

    // Navigation Properties
    public User? User { get; set; }
    public ICollection<Project>? Projects { get; set; }
    public ICollection<Invoice>? Invoices { get; set; }
} 