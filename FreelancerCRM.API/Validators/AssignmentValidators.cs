using FluentValidation;
using FreelancerCRM.API.DTOs;

namespace FreelancerCRM.API.Validators
{
    public class AssignmentCreateDtoValidator : AbstractValidator<AssignmentCreateDto>
    {
        public AssignmentCreateDtoValidator()
        {
            RuleFor(x => x.TaskName)
                .NotEmpty().WithMessage("Görev adı zorunludur")
                .MaximumLength(100).WithMessage("Görev adı en fazla 100 karakter olabilir");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("Proje seçimi zorunludur");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Kullanıcı seçimi zorunludur");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Başlangıç tarihi zorunludur")
                .Must((model, startDate) => !model.DueDate.HasValue || startDate <= model.DueDate)
                .WithMessage("Başlangıç tarihi bitiş tarihinden sonra olamaz");

            RuleFor(x => x.DueDate)
                .Must((model, dueDate) => !dueDate.HasValue || model.StartDate <= dueDate)
                .WithMessage("Bitiş tarihi başlangıç tarihinden önce olamaz")
                .When(x => x.DueDate.HasValue);

            RuleFor(x => x.EstimatedHours)
                .GreaterThanOrEqualTo(0).WithMessage("Tahmini süre 0 veya pozitif olmalıdır");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notlar en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }

    public class AssignmentUpdateDtoValidator : AbstractValidator<AssignmentUpdateDto>
    {
        public AssignmentUpdateDtoValidator()
        {
            RuleFor(x => x.TaskName)
                .NotEmpty().WithMessage("Görev adı zorunludur")
                .MaximumLength(100).WithMessage("Görev adı en fazla 100 karakter olabilir");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("Proje seçimi zorunludur");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Kullanıcı seçimi zorunludur");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Başlangıç tarihi zorunludur")
                .Must((model, startDate) => !model.DueDate.HasValue || startDate <= model.DueDate)
                .WithMessage("Başlangıç tarihi bitiş tarihinden sonra olamaz");

            RuleFor(x => x.DueDate)
                .Must((model, dueDate) => !dueDate.HasValue || model.StartDate <= dueDate)
                .WithMessage("Bitiş tarihi başlangıç tarihinden önce olamaz")
                .When(x => x.DueDate.HasValue);

            RuleFor(x => x.EstimatedHours)
                .GreaterThanOrEqualTo(0).WithMessage("Tahmini süre 0 veya pozitif olmalıdır");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notlar en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }
} 