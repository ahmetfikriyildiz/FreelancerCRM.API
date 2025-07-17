using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FreelancerCRM.API.Models;

public class Invoice : BaseEntity
{
    [Key]
    public int InvoiceID { get; set; }
    [ForeignKey("User")]
    public int UserID { get; set; }
    [ForeignKey("Client")]
    public int ClientID { get; set; }
    [ForeignKey("Project")]
    public int? ProjectID { get; set; }
    [MaxLength(50)]
    public string? InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; }
    [JsonIgnore]
    public DateTime InvoiceDate => IssueDate;
    public DateTime DueDate { get; set; }
    [MaxLength(20)]
    public string? Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal DiscountRate { get; set; } = 0;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; } = 0;
    public decimal OutstandingAmount { get; set; } = 0;
    [MaxLength(10)]
    public string Currency { get; set; } = "USD";
    [MaxLength(100)]
    public string? PaymentTerms { get; set; }
    public string? Notes { get; set; }
    public string? Description { get; set; }
    public DateTime? PaidAt { get; set; }

    // Navigation Properties
    public User? User { get; set; }
    public Client? Client { get; set; }
    public Project? Project { get; set; }
    public ICollection<InvoiceItem>? InvoiceItems { get; set; }
} 