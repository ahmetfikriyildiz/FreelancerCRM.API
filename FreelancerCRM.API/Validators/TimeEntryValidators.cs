using FluentValidation;
using FreelancerCRM.API.DTOs;

namespace FreelancerCRM.API.Validators
{
    public class TimeEntryCreateDtoValidator : AbstractValidator<TimeEntryCreateDto>
    {
        public TimeEntryCreateDtoValidator()
        {
            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("Proje seçimi zorunludur");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Kullanıcı seçimi zorunludur");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Başlangıç zamanı zorunludur")
                .Must((model, startTime) => !model.EndTime.HasValue || startTime <= model.EndTime)
                .WithMessage("Başlangıç zamanı bitiş zamanından sonra olamaz");

            RuleFor(x => x.EndTime)
                .Must((model, endTime) => !endTime.HasValue || model.StartTime <= endTime)
                .WithMessage("Bitiş zamanı başlangıç zamanından önce olamaz")
                .When(x => x.EndTime.HasValue);

            RuleFor(x => x.Duration)
                .GreaterThanOrEqualTo(0).WithMessage("Süre 0 veya pozitif olmalıdır");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.HourlyRate)
                .GreaterThanOrEqualTo(0).WithMessage("Saatlik ücret 0 veya pozitif olmalıdır");

            RuleFor(x => x.WithholdingTaxRate)
                .InclusiveBetween(0, 100).WithMessage("Stopaj oranı 0-100 arasında olmalıdır");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notlar en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }

    public class TimeEntryUpdateDtoValidator : AbstractValidator<TimeEntryUpdateDto>
    {
        public TimeEntryUpdateDtoValidator()
        {
            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("Proje seçimi zorunludur");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Kullanıcı seçimi zorunludur");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Başlangıç zamanı zorunludur")
                .Must((model, startTime) => !model.EndTime.HasValue || startTime <= model.EndTime)
                .WithMessage("Başlangıç zamanı bitiş zamanından sonra olamaz");

            RuleFor(x => x.EndTime)
                .Must((model, endTime) => !endTime.HasValue || model.StartTime <= endTime)
                .WithMessage("Bitiş zamanı başlangıç zamanından önce olamaz")
                .When(x => x.EndTime.HasValue);

            RuleFor(x => x.Duration)
                .GreaterThanOrEqualTo(0).WithMessage("Süre 0 veya pozitif olmalıdır");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.HourlyRate)
                .GreaterThanOrEqualTo(0).WithMessage("Saatlik ücret 0 veya pozitif olmalıdır");

            RuleFor(x => x.WithholdingTaxRate)
                .InclusiveBetween(0, 100).WithMessage("Stopaj oranı 0-100 arasında olmalıdır");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notlar en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }
} 