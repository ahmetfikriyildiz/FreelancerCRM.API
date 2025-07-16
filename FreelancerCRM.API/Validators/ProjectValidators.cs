using FluentValidation;
using FreelancerCRM.API.DTOs;

namespace FreelancerCRM.API.Validators
{
    public class ProjectCreateDtoValidator : AbstractValidator<ProjectCreateDto>
    {
        public ProjectCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Proje adı zorunludur")
                .MaximumLength(100).WithMessage("Proje adı en fazla 100 karakter olabilir");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.ClientId)
                .GreaterThan(0).WithMessage("Müşteri seçimi zorunludur");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Kullanıcı seçimi zorunludur");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Başlangıç tarihi zorunludur")
                .Must((model, startDate) => !model.EndDate.HasValue || startDate <= model.EndDate)
                .WithMessage("Başlangıç tarihi bitiş tarihinden sonra olamaz");

            RuleFor(x => x.EndDate)
                .Must((model, endDate) => !endDate.HasValue || model.StartDate <= endDate)
                .WithMessage("Bitiş tarihi başlangıç tarihinden önce olamaz")
                .When(x => x.EndDate.HasValue);

            RuleFor(x => x.Budget)
                .GreaterThanOrEqualTo(0).WithMessage("Bütçe 0 veya pozitif olmalıdır");

            RuleFor(x => x.HourlyRate)
                .GreaterThanOrEqualTo(0).WithMessage("Saatlik ücret 0 veya pozitif olmalıdır");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notlar en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Notes));

            RuleFor(x => x.Priority)
                .InclusiveBetween(1, 5).WithMessage("Öncelik 1-5 arasında olmalıdır");
        }
    }

    public class ProjectUpdateDtoValidator : AbstractValidator<ProjectUpdateDto>
    {
        public ProjectUpdateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Proje adı zorunludur")
                .MaximumLength(100).WithMessage("Proje adı en fazla 100 karakter olabilir");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.ClientId)
                .GreaterThan(0).WithMessage("Müşteri seçimi zorunludur");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Kullanıcı seçimi zorunludur");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Başlangıç tarihi zorunludur")
                .Must((model, startDate) => !model.EndDate.HasValue || startDate <= model.EndDate)
                .WithMessage("Başlangıç tarihi bitiş tarihinden sonra olamaz");

            RuleFor(x => x.EndDate)
                .Must((model, endDate) => !endDate.HasValue || model.StartDate <= endDate)
                .WithMessage("Bitiş tarihi başlangıç tarihinden önce olamaz")
                .When(x => x.EndDate.HasValue);

            RuleFor(x => x.Budget)
                .GreaterThanOrEqualTo(0).WithMessage("Bütçe 0 veya pozitif olmalıdır");

            RuleFor(x => x.HourlyRate)
                .GreaterThanOrEqualTo(0).WithMessage("Saatlik ücret 0 veya pozitif olmalıdır");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notlar en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Notes));

            RuleFor(x => x.Priority)
                .InclusiveBetween(1, 5).WithMessage("Öncelik 1-5 arasında olmalıdır");
        }
    }
} 