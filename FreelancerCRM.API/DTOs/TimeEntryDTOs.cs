using System.ComponentModel.DataAnnotations;
using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.DTOs
{
    public class TimeEntryCreateDto
    {
        [Required(ErrorMessage = "Proje seçimi zorunludur")]
        public int ProjectId { get; set; }

        public int? AssignmentId { get; set; }

        [Required(ErrorMessage = "Kullanıcı seçimi zorunludur")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Başlangıç zamanı zorunludur")]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Süre 0 veya pozitif olmalıdır")]
        public decimal Duration { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Description { get; set; }

        public bool IsBillable { get; set; } = true;

        [Range(0, double.MaxValue, ErrorMessage = "Saatlik ücret 0 veya pozitif olmalıdır")]
        public decimal HourlyRate { get; set; }

        [Range(0, 100, ErrorMessage = "Stopaj oranı 0-100 arasında olmalıdır")]
        public decimal WithholdingTaxRate { get; set; } = 20;

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
        public string? Notes { get; set; }
    }

    public class TimeEntryUpdateDto
    {
        [Required(ErrorMessage = "Proje seçimi zorunludur")]
        public int ProjectId { get; set; }

        public int? AssignmentId { get; set; }

        [Required(ErrorMessage = "Kullanıcı seçimi zorunludur")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Başlangıç zamanı zorunludur")]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Süre 0 veya pozitif olmalıdır")]
        public decimal Duration { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Description { get; set; }

        public bool IsBillable { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Saatlik ücret 0 veya pozitif olmalıdır")]
        public decimal HourlyRate { get; set; }

        [Range(0, 100, ErrorMessage = "Stopaj oranı 0-100 arasında olmalıdır")]
        public decimal WithholdingTaxRate { get; set; }

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
        public string? Notes { get; set; }
    }

    public class TimeEntryResponseDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int? AssignmentId { get; set; }
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal Duration { get; set; }
        public string? Description { get; set; }
        public bool IsBillable { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal WithholdingTaxRate { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal WithholdingTaxAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ProjectSummaryDto Project { get; set; } = new();
        public AssignmentSummaryDto? Assignment { get; set; }
        public UserSummaryDto User { get; set; } = new();

        // Calculated properties
        public bool IsRunning { get; set; }
        public string FormattedDuration { get; set; } = string.Empty;
    }

    public class TimeEntrySummaryDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int? AssignmentId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal Duration { get; set; }
        public string? Description { get; set; }
        public bool IsBillable { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public bool IsRunning { get; set; }
        public string FormattedDuration { get; set; } = string.Empty;
    }

    public class TimeEntrySearchDto
    {
        public int? ProjectId { get; set; }
        public int? AssignmentId { get; set; }
        public int? UserId { get; set; }
        public DateTime? StartTimeFrom { get; set; }
        public DateTime? StartTimeTo { get; set; }
        public DateTime? EndTimeFrom { get; set; }
        public DateTime? EndTimeTo { get; set; }
        public bool? IsBillable { get; set; }
        public decimal? MinDuration { get; set; }
        public decimal? MaxDuration { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool? IsRunning { get; set; }
    }

    public class TimeEntryStartDto
    {
        [Required(ErrorMessage = "Proje seçimi zorunludur")]
        public int ProjectId { get; set; }

        public int? AssignmentId { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Description { get; set; }

        public bool IsBillable { get; set; } = true;

        [Range(0, double.MaxValue, ErrorMessage = "Saatlik ücret 0 veya pozitif olmalıdır")]
        public decimal HourlyRate { get; set; }
    }

    public class TimeEntryStopDto
    {
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Description { get; set; }

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
        public string? Notes { get; set; }
    }
} 