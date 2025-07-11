using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerCRM.API.Models;

public class User
{
    [Key]
    public int UserID { get; set; }
    [Required, MaxLength(100)]
    public string Username { get; set; } = null!;
    [Required, MaxLength(255)]
    public string Email { get; set; } = null!;
    [Required]
    public string PasswordHash { get; set; } = null!;
    [MaxLength(100)]
    public string FirstName { get; set; } = null!;
    [MaxLength(100)]
    public string LastName { get; set; } = null!;
    public string? ProfilePicture { get; set; }
    [MaxLength(20)]
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    [MaxLength(50)]
    public string? Timezone { get; set; }
    // Türkiye'ye özgü alanlar
    [MaxLength(20)]
    public string? TaxNumber { get; set; }
    [MaxLength(50)]
    public string? TaxOffice { get; set; }
    [MaxLength(11)]
    public string? TCKN { get; set; }
    public bool IsKDVMukellefi { get; set; }
    // KVKK uyumluluk
    public DateTime? ConsentDate { get; set; }
    public bool DataProcessingConsent { get; set; }
    // Navigation Properties
    public ICollection<Client>? Clients { get; set; }
    public ICollection<Project>? Projects { get; set; }
    public ICollection<TimeEntry>? TimeEntries { get; set; }
    public ICollection<Invoice>? Invoices { get; set; }
} 