using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerCRM.API.Models;

public class InvoiceItem : BaseEntity
{
    [Key]
    public int InvoiceItemID { get; set; }
    [ForeignKey("Invoice")]
    public int InvoiceID { get; set; }
    [Required, MaxLength(500)]
    public string Description { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    [MaxLength(20)]
    public string? Unit { get; set; }
    public string? Notes { get; set; }

    // Navigation Properties
    public Invoice? Invoice { get; set; }
} 