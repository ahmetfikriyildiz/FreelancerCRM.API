using FluentValidation;
using FreelancerCRM.API.DTOs;

namespace FreelancerCRM.API.Validators
{
    public class InvoiceCreateDtoValidator : AbstractValidator<InvoiceCreateDto>
    {
        public InvoiceCreateDtoValidator()
        {
            RuleFor(x => x.ClientId)
                .GreaterThan(0).WithMessage("Müşteri seçimi zorunludur");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Kullanıcı seçimi zorunludur");

            RuleFor(x => x.InvoiceNumber)
                .NotEmpty().WithMessage("Fatura numarası zorunludur")
                .MaximumLength(50).WithMessage("Fatura numarası en fazla 50 karakter olabilir");

            RuleFor(x => x.InvoiceDate)
                .NotEmpty().WithMessage("Fatura tarihi zorunludur")
                .Must((model, invoiceDate) => invoiceDate <= model.DueDate)
                .WithMessage("Fatura tarihi vade tarihinden sonra olamaz");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Vade tarihi zorunludur")
                .Must((model, dueDate) => model.InvoiceDate <= dueDate)
                .WithMessage("Vade tarihi fatura tarihinden önce olamaz");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.SubTotal)
                .GreaterThanOrEqualTo(0).WithMessage("Ara toplam 0 veya pozitif olmalıdır");

            RuleFor(x => x.VATRate)
                .InclusiveBetween(0, 100).WithMessage("KDV oranı 0-100 arasında olmalıdır");

            RuleFor(x => x.VATAmount)
                .GreaterThanOrEqualTo(0).WithMessage("KDV tutarı 0 veya pozitif olmalıdır");

            RuleFor(x => x.DiscountRate)
                .InclusiveBetween(0, 100).WithMessage("İndirim oranı 0-100 arasında olmalıdır");

            RuleFor(x => x.DiscountAmount)
                .GreaterThanOrEqualTo(0).WithMessage("İndirim tutarı 0 veya pozitif olmalıdır");

            RuleFor(x => x.WithholdingTaxRate)
                .InclusiveBetween(0, 100).WithMessage("Stopaj oranı 0-100 arasında olmalıdır");
        }
    }

    public class InvoiceUpdateDtoValidator : AbstractValidator<InvoiceUpdateDto>
    {
        public InvoiceUpdateDtoValidator()
        {
            RuleFor(x => x.ClientId)
                .GreaterThan(0).WithMessage("Müşteri seçimi zorunludur");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Kullanıcı seçimi zorunludur");

            RuleFor(x => x.InvoiceNumber)
                .NotEmpty().WithMessage("Fatura numarası zorunludur")
                .MaximumLength(50).WithMessage("Fatura numarası en fazla 50 karakter olabilir");

            RuleFor(x => x.InvoiceDate)
                .NotEmpty().WithMessage("Fatura tarihi zorunludur")
                .Must((model, invoiceDate) => invoiceDate <= model.DueDate)
                .WithMessage("Fatura tarihi vade tarihinden sonra olamaz");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Vade tarihi zorunludur")
                .Must((model, dueDate) => model.InvoiceDate <= dueDate)
                .WithMessage("Vade tarihi fatura tarihinden önce olamaz");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.SubTotal)
                .GreaterThanOrEqualTo(0).WithMessage("Ara toplam 0 veya pozitif olmalıdır");

            RuleFor(x => x.VATRate)
                .InclusiveBetween(0, 100).WithMessage("KDV oranı 0-100 arasında olmalıdır");

            RuleFor(x => x.VATAmount)
                .GreaterThanOrEqualTo(0).WithMessage("KDV tutarı 0 veya pozitif olmalıdır");

            RuleFor(x => x.DiscountRate)
                .InclusiveBetween(0, 100).WithMessage("İndirim oranı 0-100 arasında olmalıdır");

            RuleFor(x => x.DiscountAmount)
                .GreaterThanOrEqualTo(0).WithMessage("İndirim tutarı 0 veya pozitif olmalıdır");

            RuleFor(x => x.WithholdingTaxRate)
                .InclusiveBetween(0, 100).WithMessage("Stopaj oranı 0-100 arasında olmalıdır");
        }
    }
} 