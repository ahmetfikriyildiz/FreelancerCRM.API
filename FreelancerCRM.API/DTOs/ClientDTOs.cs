using System.ComponentModel.DataAnnotations;

namespace FreelancerCRM.API.DTOs
{
    public class ClientCreateDto
    {
        [Required(ErrorMessage = "Şirket adı zorunludur")]
        [StringLength(100, ErrorMessage = "Şirket adı en fazla 100 karakter olabilir")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "İletişim kişisi adı zorunludur")]
        [StringLength(100, ErrorMessage = "İletişim kişisi adı en fazla 100 karakter olabilir")]
        public string ContactName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir")]
        public string Email { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "Telefon numarası en fazla 15 karakter olabilir")]
        public string? PhoneNumber { get; set; }

        [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir")]
        public string? Address { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Vergi numarası 10 haneli olmalıdır")]
        public string? TaxNumber { get; set; }

        [StringLength(100, ErrorMessage = "Vergi dairesi en fazla 100 karakter olabilir")]
        public string? TaxOffice { get; set; }

        [StringLength(100, ErrorMessage = "Sektör en fazla 100 karakter olabilir")]
        public string? Industry { get; set; }

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
        public string? Notes { get; set; }

        [Range(1, 5, ErrorMessage = "Öncelik 1-5 arasında olmalıdır")]
        public int Priority { get; set; } = 3;

        public bool IsActive { get; set; } = true;
    }

    public class ClientUpdateDto
    {
        [Required(ErrorMessage = "Şirket adı zorunludur")]
        [StringLength(100, ErrorMessage = "Şirket adı en fazla 100 karakter olabilir")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "İletişim kişisi adı zorunludur")]
        [StringLength(100, ErrorMessage = "İletişim kişisi adı en fazla 100 karakter olabilir")]
        public string ContactName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir")]
        public string Email { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "Telefon numarası en fazla 15 karakter olabilir")]
        public string? PhoneNumber { get; set; }

        [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir")]
        public string? Address { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Vergi numarası 10 haneli olmalıdır")]
        public string? TaxNumber { get; set; }

        [StringLength(100, ErrorMessage = "Vergi dairesi en fazla 100 karakter olabilir")]
        public string? TaxOffice { get; set; }

        [StringLength(100, ErrorMessage = "Sektör en fazla 100 karakter olabilir")]
        public string? Industry { get; set; }

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
        public string? Notes { get; set; }

        [Range(1, 5, ErrorMessage = "Öncelik 1-5 arasında olmalıdır")]
        public int Priority { get; set; }

        public bool IsActive { get; set; }
    }

    public class ClientResponseDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? TaxNumber { get; set; }
        public string? TaxOffice { get; set; }
        public string? Industry { get; set; }
        public string? Notes { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchived { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ArchivedAt { get; set; }

        // Navigation properties
        public List<ProjectSummaryDto> Projects { get; set; } = new();
        public List<InvoiceSummaryDto> Invoices { get; set; } = new();
    }

    public class ClientSummaryDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchived { get; set; }
    }

    public class ClientSearchDto
    {
        public string? CompanyName { get; set; }
        public string? ContactName { get; set; }
        public string? Email { get; set; }
        public string? Industry { get; set; }
        public int? Priority { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsArchived { get; set; }
    }
} 