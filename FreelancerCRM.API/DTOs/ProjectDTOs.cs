using System.ComponentModel.DataAnnotations;
using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.DTOs
{
    public class ProjectCreateDto
    {
        [Required(ErrorMessage = "Proje adı zorunludur")]
        [StringLength(100, ErrorMessage = "Proje adı en fazla 100 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Müşteri seçimi zorunludur")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Kullanıcı seçimi zorunludur")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Başlangıç tarihi zorunludur")]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Bütçe 0 veya pozitif olmalıdır")]
        public decimal Budget { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Saatlik ücret 0 veya pozitif olmalıdır")]
        public decimal HourlyRate { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Planning;

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
        public string? Notes { get; set; }

        [Range(1, 5, ErrorMessage = "Öncelik 1-5 arasında olmalıdır")]
        public int Priority { get; set; } = 3;

        public bool IsActive { get; set; } = true;
    }

    public class ProjectUpdateDto
    {
        [Required(ErrorMessage = "Proje adı zorunludur")]
        [StringLength(100, ErrorMessage = "Proje adı en fazla 100 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Müşteri seçimi zorunludur")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Kullanıcı seçimi zorunludur")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Başlangıç tarihi zorunludur")]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Bütçe 0 veya pozitif olmalıdır")]
        public decimal Budget { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Saatlik ücret 0 veya pozitif olmalıdır")]
        public decimal HourlyRate { get; set; }

        public ProjectStatus Status { get; set; }

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
        public string? Notes { get; set; }

        [Range(1, 5, ErrorMessage = "Öncelik 1-5 arasında olmalıdır")]
        public int Priority { get; set; }

        public bool IsActive { get; set; }
    }

    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ClientId { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Budget { get; set; }
        public decimal HourlyRate { get; set; }
        public ProjectStatus Status { get; set; }
        public string? Notes { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ClientSummaryDto Client { get; set; } = new();
        public UserSummaryDto User { get; set; } = new();
        public List<AssignmentSummaryDto> Assignments { get; set; } = new();
        public List<TimeEntrySummaryDto> TimeEntries { get; set; } = new();

        // Calculated properties
        public decimal TotalTimeSpent { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal RemainingBudget { get; set; }
        public int CompletionPercentage { get; set; }
    }

    public class ProjectSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Budget { get; set; }
        public decimal HourlyRate { get; set; }
        public ProjectStatus Status { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalTimeSpent { get; set; }
        public decimal TotalEarnings { get; set; }
        public int CompletionPercentage { get; set; }
    }

    public class ProjectSearchDto
    {
        public string? Name { get; set; }
        public int? ClientId { get; set; }
        public int? UserId { get; set; }
        public ProjectStatus? Status { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public decimal? MinBudget { get; set; }
        public decimal? MaxBudget { get; set; }
        public int? Priority { get; set; }
        public bool? IsActive { get; set; }
    }
} 