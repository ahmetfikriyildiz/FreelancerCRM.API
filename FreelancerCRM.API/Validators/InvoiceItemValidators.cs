using FluentValidation;
using FreelancerCRM.API.DTOs;

namespace FreelancerCRM.API.Validators
{
    public class InvoiceItemCreateDtoValidator : AbstractValidator<InvoiceItemCreateDto>
    {
        public InvoiceItemCreateDtoValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama zorunludur")
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Miktar 0'dan büyük olmalıdır");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Birim fiyat 0'dan büyük olmalıdır");

            RuleFor(x => x.TotalPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Toplam tutar 0 veya pozitif olmalıdır")
                .Must((model, totalPrice) => totalPrice == model.Quantity * model.UnitPrice)
                .WithMessage("Toplam tutar, miktar ile birim fiyatın çarpımına eşit olmalıdır");

            RuleFor(x => x.Unit)
                .MaximumLength(20).WithMessage("Birim en fazla 20 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Unit));

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notlar en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }

    public class InvoiceItemUpdateDtoValidator : AbstractValidator<InvoiceItemUpdateDto>
    {
        public InvoiceItemUpdateDtoValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama zorunludur")
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Miktar 0'dan büyük olmalıdır");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Birim fiyat 0'dan büyük olmalıdır");

            RuleFor(x => x.TotalPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Toplam tutar 0 veya pozitif olmalıdır")
                .Must((model, totalPrice) => totalPrice == model.Quantity * model.UnitPrice)
                .WithMessage("Toplam tutar, miktar ile birim fiyatın çarpımına eşit olmalıdır");

            RuleFor(x => x.Unit)
                .MaximumLength(20).WithMessage("Birim en fazla 20 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Unit));

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notlar en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }
} 