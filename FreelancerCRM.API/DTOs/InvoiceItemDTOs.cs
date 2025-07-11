using System.ComponentModel.DataAnnotations;

namespace FreelancerCRM.API.DTOs
{
    public class InvoiceItemCreateDto
    {
        [Required(ErrorMessage = "Açıklama zorunludur")]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Miktar zorunludur")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Miktar 0'dan büyük olmalıdır")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "Birim fiyat zorunludur")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Birim fiyat 0'dan büyük olmalıdır")]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Toplam tutar 0 veya pozitif olmalıdır")]
        public decimal TotalPrice { get; set; }

        [StringLength(20, ErrorMessage = "Birim en fazla 20 karakter olabilir")]
        public string? Unit { get; set; }

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
        public string? Notes { get; set; }
    }

    public class InvoiceItemUpdateDto
    {
        [Required(ErrorMessage = "Açıklama zorunludur")]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Miktar zorunludur")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Miktar 0'dan büyük olmalıdır")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "Birim fiyat zorunludur")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Birim fiyat 0'dan büyük olmalıdır")]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Toplam tutar 0 veya pozitif olmalıdır")]
        public decimal TotalPrice { get; set; }

        [StringLength(20, ErrorMessage = "Birim en fazla 20 karakter olabilir")]
        public string? Unit { get; set; }

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
        public string? Notes { get; set; }
    }

    public class InvoiceItemResponseDto
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Unit { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Calculated properties
        public string FormattedQuantity { get; set; } = string.Empty;
        public string FormattedUnitPrice { get; set; } = string.Empty;
        public string FormattedTotalPrice { get; set; } = string.Empty;
    }

    public class InvoiceItemSummaryDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Unit { get; set; }
    }
} 