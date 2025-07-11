using System.ComponentModel.DataAnnotations;
using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.DTOs
{
    public class AssignmentCreateDto
    {
        [Required(ErrorMessage = "Görev adı zorunludur")]
        [StringLength(100, ErrorMessage = "Görev adı en fazla 100 karakter olabilir")]
        public string TaskName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Proje seçimi zorunludur")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Kullanıcı seçimi zorunludur")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Başlangıç tarihi zorunludur")]
        public DateTime StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tahmini süre 0 veya pozitif olmalıdır")]
        public decimal EstimatedHours { get; set; }

        public AssignmentStatus Status { get; set; } = AssignmentStatus.NotStarted;

        public Priority Priority { get; set; } = Priority.Medium;

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class AssignmentUpdateDto
    {
        [Required(ErrorMessage = "Görev adı zorunludur")]
        [StringLength(100, ErrorMessage = "Görev adı en fazla 100 karakter olabilir")]
        public string TaskName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Proje seçimi zorunludur")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Kullanıcı seçimi zorunludur")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Başlangıç tarihi zorunludur")]
        public DateTime StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tahmini süre 0 veya pozitif olmalıdır")]
        public decimal EstimatedHours { get; set; }

        public AssignmentStatus Status { get; set; }

        public Priority Priority { get; set; }

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
        public string? Notes { get; set; }

        public bool IsActive { get; set; }
    }

    public class AssignmentResponseDto
    {
        public int Id { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal EstimatedHours { get; set; }
        public decimal ActualHours { get; set; }
        public AssignmentStatus Status { get; set; }
        public Priority Priority { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        public ProjectSummaryDto Project { get; set; } = new();
        public UserSummaryDto User { get; set; } = new();
        public List<TimeEntrySummaryDto> TimeEntries { get; set; } = new();

        // Calculated properties
        public int CompletionPercentage { get; set; }
        public bool IsOverdue { get; set; }
        public int DaysRemaining { get; set; }
    }

    public class AssignmentSummaryDto
    {
        public int Id { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal EstimatedHours { get; set; }
        public decimal ActualHours { get; set; }
        public AssignmentStatus Status { get; set; }
        public Priority Priority { get; set; }
        public bool IsActive { get; set; }
        public int CompletionPercentage { get; set; }
        public bool IsOverdue { get; set; }
    }

    public class AssignmentSearchDto
    {
        public string? TaskName { get; set; }
        public int? ProjectId { get; set; }
        public int? UserId { get; set; }
        public AssignmentStatus? Status { get; set; }
        public Priority? Priority { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsOverdue { get; set; }
    }
} 