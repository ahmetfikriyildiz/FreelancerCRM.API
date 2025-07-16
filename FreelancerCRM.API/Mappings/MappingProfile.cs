using AutoMapper;
using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateUserMappings();
            CreateClientMappings();
            CreateProjectMappings();
            CreateAssignmentMappings();
            CreateTimeEntryMappings();
            CreateInvoiceMappings();
            CreateInvoiceItemMappings();
        }

        private void CreateUserMappings()
        {
            // User Entity to DTOs
            CreateMap<User, UserResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<User, UserSummaryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            // DTOs to User Entity
            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password)); // Bu service'de hash'lenecek

            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Password güncelleme ayrı method'da
        }

        private void CreateClientMappings()
        {
            // Client Entity to DTOs
            CreateMap<Client, ClientResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ClientID))
                .ForMember(dest => dest.Projects, opt => opt.MapFrom(src => src.Projects))
                .ForMember(dest => dest.Invoices, opt => opt.MapFrom(src => src.Invoices));

            CreateMap<Client, ClientSummaryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ClientID));

            // DTOs to Client Entity
            CreateMap<ClientCreateDto, Client>()
                .ForMember(dest => dest.ClientID, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsArchived, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.ArchivedAt, opt => opt.Ignore());

            CreateMap<ClientUpdateDto, Client>()
                .ForMember(dest => dest.ClientID, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsArchived, opt => opt.Ignore())
                .ForMember(dest => dest.ArchivedAt, opt => opt.Ignore());
        }

        private void CreateProjectMappings()
        {
            // Project Entity to DTOs
            CreateMap<Project, ProjectResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProjectID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProjectName))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientID))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserID))
                .ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Client))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Assignments, opt => opt.MapFrom(src => src.Tasks))
                .ForMember(dest => dest.TimeEntries, opt => opt.MapFrom(src => src.TimeEntries))
                .ForMember(dest => dest.TotalTimeSpent, opt => opt.MapFrom(src => src.ActualHours))
                .ForMember(dest => dest.TotalEarnings, opt => opt.MapFrom(src => src.ActualCost))
                .ForMember(dest => dest.RemainingBudget, opt => opt.MapFrom(src => src.Budget - src.ActualCost))
                .ForMember(dest => dest.CompletionPercentage, opt => opt.MapFrom(src => 
                    src.EstimatedHours > 0 ? (int)((src.ActualHours / src.EstimatedHours) * 100) : 0))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ParseProjectStatus(src.Status)))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => ParsePriority(src.Priority)));

            CreateMap<Project, ProjectSummaryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProjectID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProjectName))
                .ForMember(dest => dest.TotalTimeSpent, opt => opt.MapFrom(src => src.ActualHours))
                .ForMember(dest => dest.TotalEarnings, opt => opt.MapFrom(src => src.ActualCost))
                .ForMember(dest => dest.CompletionPercentage, opt => opt.MapFrom(src => 
                    src.EstimatedHours > 0 ? (int)((src.ActualHours / src.EstimatedHours) * 100) : 0))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ParseProjectStatus(src.Status)))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => ParsePriority(src.Priority)));

            // DTOs to Project Entity
            CreateMap<ProjectCreateDto, Project>()
                .ForMember(dest => dest.ProjectID, opt => opt.Ignore())
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ClientID, opt => opt.MapFrom(src => src.ClientId))
                .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.EstimatedHours, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.ActualHours, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.ActualCost, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(src => "Hourly"))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<ProjectUpdateDto, Project>()
                .ForMember(dest => dest.ProjectID, opt => opt.Ignore())
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ClientID, opt => opt.MapFrom(src => src.ClientId))
                .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.EstimatedHours, opt => opt.Ignore())
                .ForMember(dest => dest.ActualHours, opt => opt.Ignore())
                .ForMember(dest => dest.ActualCost, opt => opt.Ignore())
                .ForMember(dest => dest.ContractType, opt => opt.Ignore());
        }

        private void CreateAssignmentMappings()
        {
            // Assignment Entity to DTOs
            CreateMap<Assignment, AssignmentResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TaskID))
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectID))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserID))
                .ForMember(dest => dest.Project, opt => opt.MapFrom(src => src.Project))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.TimeEntries, opt => opt.MapFrom(src => src.TimeEntries))
                .ForMember(dest => dest.CompletionPercentage, opt => opt.MapFrom(src => 
                    src.EstimatedHours > 0 ? (int)((src.ActualHours / src.EstimatedHours) * 100) : 0))
                .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => 
                    src.DueDate.HasValue && src.DueDate.Value < DateTime.UtcNow && src.Status != "Completed"))
                .ForMember(dest => dest.DaysRemaining, opt => opt.MapFrom(src => 
                    src.DueDate.HasValue ? (int)(src.DueDate.Value - DateTime.UtcNow).TotalDays : 0))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ParseAssignmentStatus(src.Status)))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => ParsePriority(src.Priority)));

            CreateMap<Assignment, AssignmentSummaryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TaskID))
                .ForMember(dest => dest.CompletionPercentage, opt => opt.MapFrom(src => 
                    src.EstimatedHours > 0 ? (int)((src.ActualHours / src.EstimatedHours) * 100) : 0))
                .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => 
                    src.DueDate.HasValue && src.DueDate.Value < DateTime.UtcNow && src.Status != "Completed"))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ParseAssignmentStatus(src.Status)))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => ParsePriority(src.Priority)));

            // DTOs to Assignment Entity
            CreateMap<AssignmentCreateDto, Assignment>()
                .ForMember(dest => dest.TaskID, opt => opt.Ignore())
                .ForMember(dest => dest.ProjectID, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ActualHours, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CompletedAt, opt => opt.Ignore());

            CreateMap<AssignmentUpdateDto, Assignment>()
                .ForMember(dest => dest.TaskID, opt => opt.Ignore())
                .ForMember(dest => dest.ProjectID, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ActualHours, opt => opt.Ignore())
                .ForMember(dest => dest.CompletedAt, opt => opt.Ignore());
        }

        private void CreateTimeEntryMappings()
        {
            // TimeEntry Entity to DTOs
            CreateMap<TimeEntry, TimeEntryResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TimeEntryID))
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectID))
                .ForMember(dest => dest.AssignmentId, opt => opt.MapFrom(src => src.TaskID))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserID))
                .ForMember(dest => dest.Project, opt => opt.MapFrom(src => src.Project))
                .ForMember(dest => dest.Assignment, opt => opt.MapFrom(src => src.Task))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.IsRunning, opt => opt.MapFrom(src => !src.EndTime.HasValue))
                .ForMember(dest => dest.FormattedDuration, opt => opt.MapFrom(src => FormatDuration(src.Duration)));

            CreateMap<TimeEntry, TimeEntrySummaryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TimeEntryID))
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectID))
                .ForMember(dest => dest.AssignmentId, opt => opt.MapFrom(src => src.TaskID))
                .ForMember(dest => dest.IsRunning, opt => opt.MapFrom(src => !src.EndTime.HasValue))
                .ForMember(dest => dest.FormattedDuration, opt => opt.MapFrom(src => FormatDuration(src.Duration)));

            // DTOs to TimeEntry Entity
            CreateMap<TimeEntryCreateDto, TimeEntry>()
                .ForMember(dest => dest.TimeEntryID, opt => opt.Ignore())
                .ForMember(dest => dest.ProjectID, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dest => dest.TaskID, opt => opt.MapFrom(src => src.AssignmentId))
                .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<TimeEntryUpdateDto, TimeEntry>()
                .ForMember(dest => dest.TimeEntryID, opt => opt.Ignore())
                .ForMember(dest => dest.ProjectID, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dest => dest.TaskID, opt => opt.MapFrom(src => src.AssignmentId))
                .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }

        private void CreateInvoiceMappings()
        {
            // Invoice Entity to DTOs
            CreateMap<Invoice, InvoiceResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.InvoiceID))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientID))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserID))
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectID))
                .ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Client))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Project, opt => opt.MapFrom(src => src.Project))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.InvoiceItems))
                .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => 
                    src.DueDate < DateTime.UtcNow && src.Status != "Paid"))
                .ForMember(dest => dest.DaysOverdue, opt => opt.MapFrom(src => 
                    src.DueDate < DateTime.UtcNow && src.Status != "Paid" ? (int)(DateTime.UtcNow - src.DueDate).TotalDays : 0))
                .ForMember(dest => dest.DaysUntilDue, opt => opt.MapFrom(src => 
                    src.DueDate > DateTime.UtcNow ? (int)(src.DueDate - DateTime.UtcNow).TotalDays : 0))
                .ForMember(dest => dest.FormattedInvoiceNumber, opt => opt.MapFrom(src => src.InvoiceNumber))
                .ForMember(dest => dest.StatusText, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ParseInvoiceStatus(src.Status)));

            CreateMap<Invoice, InvoiceSummaryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.InvoiceID))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientID))
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectID))
                .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => 
                    src.DueDate < DateTime.UtcNow && src.Status != "Paid"))
                .ForMember(dest => dest.DaysOverdue, opt => opt.MapFrom(src => 
                    src.DueDate < DateTime.UtcNow && src.Status != "Paid" ? (int)(DateTime.UtcNow - src.DueDate).TotalDays : 0))
                .ForMember(dest => dest.StatusText, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ParseInvoiceStatus(src.Status)));

            // DTOs to Invoice Entity
            CreateMap<InvoiceCreateDto, Invoice>()
                .ForMember(dest => dest.InvoiceID, opt => opt.Ignore())
                .ForMember(dest => dest.ClientID, opt => opt.MapFrom(src => src.ClientId))
                .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ProjectID, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dest => dest.PaidAmount, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.OutstandingAmount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.PaidAt, opt => opt.Ignore());

            CreateMap<InvoiceUpdateDto, Invoice>()
                .ForMember(dest => dest.InvoiceID, opt => opt.Ignore())
                .ForMember(dest => dest.ClientID, opt => opt.MapFrom(src => src.ClientId))
                .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ProjectID, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.PaidAmount, opt => opt.Ignore())
                .ForMember(dest => dest.OutstandingAmount, opt => opt.Ignore())
                .ForMember(dest => dest.PaidAt, opt => opt.Ignore());
        }

        private void CreateInvoiceItemMappings()
        {
            // InvoiceItem Entity to DTOs
            CreateMap<InvoiceItem, InvoiceItemResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.InvoiceItemID))
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.InvoiceID))
                .ForMember(dest => dest.FormattedQuantity, opt => opt.MapFrom(src => src.Quantity.ToString("N2")))
                .ForMember(dest => dest.FormattedUnitPrice, opt => opt.MapFrom(src => src.UnitPrice.ToString("C2")))
                .ForMember(dest => dest.FormattedTotalPrice, opt => opt.MapFrom(src => src.TotalPrice.ToString("C2")));

            CreateMap<InvoiceItem, InvoiceItemSummaryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.InvoiceItemID));

            // DTOs to InvoiceItem Entity
            CreateMap<InvoiceItemCreateDto, InvoiceItem>()
                .ForMember(dest => dest.InvoiceItemID, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceID, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<InvoiceItemUpdateDto, InvoiceItem>()
                .ForMember(dest => dest.InvoiceItemID, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceID, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }

        // Helper methods for parsing enums from strings
        private static ProjectStatus ParseProjectStatus(string? status)
        {
            return status switch
            {
                "Planning" => ProjectStatus.Planning,
                "InProgress" => ProjectStatus.InProgress,
                "OnHold" => ProjectStatus.OnHold,
                "Completed" => ProjectStatus.Completed,
                "Cancelled" => ProjectStatus.Cancelled,
                _ => ProjectStatus.Planning
            };
        }

        private static InvoiceStatus ParseInvoiceStatus(string? status)
        {
            return status switch
            {
                "Draft" => InvoiceStatus.Draft,
                "Sent" => InvoiceStatus.Sent,
                "Paid" => InvoiceStatus.Paid,
                "Overdue" => InvoiceStatus.Overdue,
                "Cancelled" => InvoiceStatus.Cancelled,
                _ => InvoiceStatus.Draft
            };
        }

        private static AssignmentStatus ParseAssignmentStatus(string? status)
        {
            return status switch
            {
                "NotStarted" => AssignmentStatus.NotStarted,
                "InProgress" => AssignmentStatus.InProgress,
                "Completed" => AssignmentStatus.Completed,
                "OnHold" => AssignmentStatus.OnHold,
                "Cancelled" => AssignmentStatus.Cancelled,
                _ => AssignmentStatus.NotStarted
            };
        }

        private static Priority ParsePriority(string? priority)
        {
            return priority switch
            {
                "Low" => Priority.Low,
                "Medium" => Priority.Medium,
                "High" => Priority.High,
                "Critical" => Priority.Critical,
                "Urgent" => Priority.Urgent,
                _ => Priority.Medium
            };
        }

        private static string FormatDuration(decimal duration)
        {
            var hours = (int)duration;
            var minutes = (int)((duration - hours) * 60);
            return $"{hours:D2}:{minutes:D2}";
        }
    }
} 