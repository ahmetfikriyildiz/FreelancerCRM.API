using FluentValidation;
using FreelancerCRM.API.DTOs;

namespace FreelancerCRM.API.Validators
{
    public class ClientCreateDtoValidator : AbstractValidator<ClientCreateDto>
    {
        public ClientCreateDtoValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Şirket adı zorunludur")
                .MaximumLength(100).WithMessage("Şirket adı en fazla 100 karakter olabilir");

            RuleFor(x => x.ContactName)
                .NotEmpty().WithMessage("İletişim kişisi adı zorunludur")
                .MaximumLength(100).WithMessage("İletişim kişisi adı en fazla 100 karakter olabilir");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta alanı zorunludur")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz")
                .MaximumLength(100).WithMessage("E-posta en fazla 100 karakter olabilir");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(15).WithMessage("Telefon numarası en fazla 15 karakter olabilir")
                .Matches(@"^[0-9+\-\s]*$").WithMessage("Telefon numarası sadece rakam, +, - ve boşluk içerebilir")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("Adres en fazla 200 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Address));

            RuleFor(x => x.TaxNumber)
                .Length(10).WithMessage("Vergi numarası 10 haneli olmalıdır")
                .Matches(@"^[0-9]*$").WithMessage("Vergi numarası sadece rakam içerebilir")
                .When(x => !string.IsNullOrEmpty(x.TaxNumber));

            RuleFor(x => x.TaxOffice)
                .MaximumLength(100).WithMessage("Vergi dairesi en fazla 100 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.TaxOffice));

            RuleFor(x => x.Industry)
                .MaximumLength(100).WithMessage("Sektör en fazla 100 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Industry));

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notlar en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Notes));

            RuleFor(x => x.Priority)
                .InclusiveBetween(1, 5).WithMessage("Öncelik 1-5 arasında olmalıdır");
        }
    }

    public class ClientUpdateDtoValidator : AbstractValidator<ClientUpdateDto>
    {
        public ClientUpdateDtoValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Şirket adı zorunludur")
                .MaximumLength(100).WithMessage("Şirket adı en fazla 100 karakter olabilir");

            RuleFor(x => x.ContactName)
                .NotEmpty().WithMessage("İletişim kişisi adı zorunludur")
                .MaximumLength(100).WithMessage("İletişim kişisi adı en fazla 100 karakter olabilir");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta alanı zorunludur")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz")
                .MaximumLength(100).WithMessage("E-posta en fazla 100 karakter olabilir");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(15).WithMessage("Telefon numarası en fazla 15 karakter olabilir")
                .Matches(@"^[0-9+\-\s]*$").WithMessage("Telefon numarası sadece rakam, +, - ve boşluk içerebilir")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("Adres en fazla 200 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Address));

            RuleFor(x => x.TaxNumber)
                .Length(10).WithMessage("Vergi numarası 10 haneli olmalıdır")
                .Matches(@"^[0-9]*$").WithMessage("Vergi numarası sadece rakam içerebilir")
                .When(x => !string.IsNullOrEmpty(x.TaxNumber));

            RuleFor(x => x.TaxOffice)
                .MaximumLength(100).WithMessage("Vergi dairesi en fazla 100 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.TaxOffice));

            RuleFor(x => x.Industry)
                .MaximumLength(100).WithMessage("Sektör en fazla 100 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Industry));

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notlar en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Notes));

            RuleFor(x => x.Priority)
                .InclusiveBetween(1, 5).WithMessage("Öncelik 1-5 arasında olmalıdır");
        }
    }
} 