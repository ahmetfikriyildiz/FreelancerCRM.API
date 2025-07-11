using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerCRM.API.Models;

public class Invoice
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
    public DateTime InvoiceDate => IssueDate; // Mapping için alias
    public DateTime DueDate { get; set; }
    [MaxLength(20)]
    public string? Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal SubTotal => Subtotal; // Mapping için alias
    public decimal TaxAmount { get; set; }
    public decimal VATAmount => TaxAmount; // Mapping için alias
    public decimal TaxRate { get; set; }
    public decimal VATRate => TaxRate; // Mapping için alias
    public decimal DiscountAmount { get; set; }
    public decimal DiscountRate { get; set; } = 0;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; } = 0;
    public decimal OutstandingAmount { get; set; } = 0;
    [MaxLength(10)]
    public string Currency { get; set; } = "TRY";
    [MaxLength(100)]
    public string? PaymentTerms { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    // Türkiye'ye özgü e-fatura alanları
    [MaxLength(50)]
    public string? EInvoiceUUID { get; set; }
    [MaxLength(20)]
    public string? EInvoiceType { get; set; }
    [MaxLength(20)]
    public string? GIBStatus { get; set; }
    public DateTime? SendDate { get; set; }
    [MaxLength(20)]
    public string? ResponseCode { get; set; }
    // Stopaj ve vergi alanları
    public decimal StopajRate { get; set; }
    public decimal StopajAmount { get; set; }
    public decimal WithholdingTaxRate => StopajRate * 100; // Yüzde olarak
    public decimal WithholdingTaxAmount => StopajAmount; // Mapping için alias
    public decimal NetAmount { get; set; }
    public string? Description { get; set; }
    // Navigation Properties
    public User? User { get; set; }
    public Client? Client { get; set; }
    public Project? Project { get; set; }
    public ICollection<InvoiceItem>? InvoiceItems { get; set; }
} 