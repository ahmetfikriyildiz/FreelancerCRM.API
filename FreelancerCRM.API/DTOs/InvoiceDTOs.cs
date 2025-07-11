using System.ComponentModel.DataAnnotations;
using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.DTOs
{
    public class InvoiceCreateDto
    {
        [Required(ErrorMessage = "Müşteri seçimi zorunludur")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Kullanıcı seçimi zorunludur")]
        public int UserId { get; set; }

        public int? ProjectId { get; set; }

        [Required(ErrorMessage = "Fatura numarası zorunludur")]
        [StringLength(50, ErrorMessage = "Fatura numarası en fazla 50 karakter olabilir")]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fatura tarihi zorunludur")]
        public DateTime InvoiceDate { get; set; }

        [Required(ErrorMessage = "Vade tarihi zorunludur")]
        public DateTime DueDate { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Ara toplam 0 veya pozitif olmalıdır")]
        public decimal SubTotal { get; set; }

        [Range(0, 100, ErrorMessage = "KDV oranı 0-100 arasında olmalıdır")]
        public decimal VATRate { get; set; } = 20;

        [Range(0, double.MaxValue, ErrorMessage = "KDV tutarı 0 veya pozitif olmalıdır")]
        public decimal VATAmount { get; set; }

        [Range(0, 100, ErrorMessage = "İndirim oranı 0-100 arasında olmalıdır")]
        public decimal DiscountRate { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "İndirim tutarı 0 veya pozitif olmalıdır")]
        public decimal DiscountAmount { get; set; }

        [Range(0, 100, ErrorMessage = "Stopaj oranı 0-100 arasında olmalıdır")]
        public decimal WithholdingTaxRate { get; set; } = 20;

        [Range(0, double.MaxValue, ErrorMessage = "Stopaj tutarı 0 veya pozitif olmalıdır")]
        public decimal WithholdingTaxAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Toplam tutar 0 veya pozitif olmalıdır")]
        public decimal TotalAmount { get; set; }

        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
        public string? Notes { get; set; }

        [StringLength(100, ErrorMessage = "E-Fatura UUID en fazla 100 karakter olabilir")]
        public string? EInvoiceUUID { get; set; }

        [StringLength(50, ErrorMessage = "GIB durumu en fazla 50 karakter olabilir")]
        public string? GIBStatus { get; set; }

        // Invoice items
        public List<InvoiceItemCreateDto> Items { get; set; } = new();
    }

    public class InvoiceUpdateDto
    {
        [Required(ErrorMessage = "Müşteri seçimi zorunludur")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Kullanıcı seçimi zorunludur")]
        public int UserId { get; set; }

        public int? ProjectId { get; set; }

        [Required(ErrorMessage = "Fatura numarası zorunludur")]
        [StringLength(50, ErrorMessage = "Fatura numarası en fazla 50 karakter olabilir")]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fatura tarihi zorunludur")]
        public DateTime InvoiceDate { get; set; }

        [Required(ErrorMessage = "Vade tarihi zorunludur")]
        public DateTime DueDate { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Ara toplam 0 veya pozitif olmalıdır")]
        public decimal SubTotal { get; set; }

        [Range(0, 100, ErrorMessage = "KDV oranı 0-100 arasında olmalıdır")]
        public decimal VATRate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "KDV tutarı 0 veya pozitif olmalıdır")]
        public decimal VATAmount { get; set; }

        [Range(0, 100, ErrorMessage = "İndirim oranı 0-100 arasında olmalıdır")]
        public decimal DiscountRate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "İndirim tutarı 0 veya pozitif olmalıdır")]
        public decimal DiscountAmount { get; set; }

        [Range(0, 100, ErrorMessage = "Stopaj oranı 0-100 arasında olmalıdır")]
        public decimal WithholdingTaxRate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Stopaj tutarı 0 veya pozitif olmalıdır")]
        public decimal WithholdingTaxAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Toplam tutar 0 veya pozitif olmalıdır")]
        public decimal TotalAmount { get; set; }

        public InvoiceStatus Status { get; set; }

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
        public string? Notes { get; set; }

        [StringLength(100, ErrorMessage = "E-Fatura UUID en fazla 100 karakter olabilir")]
        public string? EInvoiceUUID { get; set; }

        [StringLength(50, ErrorMessage = "GIB durumu en fazla 50 karakter olabilir")]
        public string? GIBStatus { get; set; }
    }

    public class InvoiceResponseDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int UserId { get; set; }
        public int? ProjectId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Description { get; set; }
        public decimal SubTotal { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal WithholdingTaxRate { get; set; }
        public decimal WithholdingTaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public string? Notes { get; set; }
        public string? EInvoiceUUID { get; set; }
        public string? GIBStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PaidAt { get; set; }

        // Navigation properties
        public ClientSummaryDto Client { get; set; } = new();
        public UserSummaryDto User { get; set; } = new();
        public ProjectSummaryDto? Project { get; set; }
        public List<InvoiceItemResponseDto> Items { get; set; } = new();

        // Calculated properties
        public bool IsOverdue { get; set; }
        public int DaysOverdue { get; set; }
        public int DaysUntilDue { get; set; }
        public string FormattedInvoiceNumber { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
    }

    public class InvoiceSummaryDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int? ProjectId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public bool IsOverdue { get; set; }
        public int DaysOverdue { get; set; }
        public string StatusText { get; set; } = string.Empty;
    }

    public class InvoiceSearchDto
    {
        public string? InvoiceNumber { get; set; }
        public int? ClientId { get; set; }
        public int? UserId { get; set; }
        public int? ProjectId { get; set; }
        public InvoiceStatus? Status { get; set; }
        public DateTime? InvoiceDateFrom { get; set; }
        public DateTime? InvoiceDateTo { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool? IsOverdue { get; set; }
        public bool? IsPaid { get; set; }
    }

    public class InvoiceStatusUpdateDto
    {
        [Required(ErrorMessage = "Durum seçimi zorunludur")]
        public InvoiceStatus Status { get; set; }

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
        public string? Notes { get; set; }
    }

    public class InvoicePaymentDto
    {
        [Required(ErrorMessage = "Ödeme tutarı zorunludur")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ödeme tutarı 0'dan büyük olmalıdır")]
        public decimal PaymentAmount { get; set; }

        [Required(ErrorMessage = "Ödeme tarihi zorunludur")]
        public DateTime PaymentDate { get; set; }

        [StringLength(500, ErrorMessage = "Ödeme notları en fazla 500 karakter olabilir")]
        public string? PaymentNotes { get; set; }
    }
} 