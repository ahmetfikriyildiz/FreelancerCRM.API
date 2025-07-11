using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerCRM.API.Models;

public class InvoiceItem
{
    [Key]
    public int InvoiceItemID { get; set; }
    [ForeignKey("Invoice")]
    public int InvoiceID { get; set; }
    [MaxLength(255)]
    public string? Description { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    public decimal TotalPrice => Amount; // Mapping için alias
    [MaxLength(20)]
    public string? ItemType { get; set; }
    [MaxLength(20)]
    public string? Unit { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    // Navigation Properties
    public Invoice? Invoice { get; set; }
} 