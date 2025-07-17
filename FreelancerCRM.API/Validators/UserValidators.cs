using FluentValidation;
using FreelancerCRM.API.DTOs;

namespace FreelancerCRM.API.Validators
{
    public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad alanı zorunludur")
                .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad alanı zorunludur")
                .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta alanı zorunludur")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz")
                .MaximumLength(100).WithMessage("E-posta en fazla 100 karakter olabilir");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre alanı zorunludur")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır")
                .MaximumLength(100).WithMessage("Şifre en fazla 100 karakter olabilir")
                .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir")
                .Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir")
                .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir")
                .Matches("[^a-zA-Z0-9]").WithMessage("Şifre en az bir özel karakter içermelidir");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(15).WithMessage("Telefon numarası en fazla 15 karakter olabilir")
                .Matches(@"^[0-9+\-\s]*$").WithMessage("Telefon numarası sadece rakam, +, - ve boşluk içerebilir")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
        }
    }

    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad alanı zorunludur")
                .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad alanı zorunludur")
                .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta alanı zorunludur")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz")
                .MaximumLength(100).WithMessage("E-posta en fazla 100 karakter olabilir");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(15).WithMessage("Telefon numarası en fazla 15 karakter olabilir")
                .Matches(@"^[0-9+\-\s]*$").WithMessage("Telefon numarası sadece rakam, +, - ve boşluk içerebilir")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.TCKN)
                .Length(11).WithMessage("TCKN 11 haneli olmalıdır")
                .Matches(@"^[0-9]*$").WithMessage("TCKN sadece rakam içerebilir")
                .When(x => !string.IsNullOrEmpty(x.TCKN));

            RuleFor(x => x.TaxNumber)
                .Length(10).WithMessage("Vergi numarası 10 haneli olmalıdır")
                .Matches(@"^[0-9]*$").WithMessage("Vergi numarası sadece rakam içerebilir")
                .When(x => !string.IsNullOrEmpty(x.TaxNumber));

            RuleFor(x => x.TaxOffice)
                .MaximumLength(100).WithMessage("Vergi dairesi en fazla 100 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.TaxOffice));

            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("Adres en fazla 200 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Address));
        }
    }

    public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
    {
        public UserLoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta alanı zorunludur")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre alanı zorunludur");
        }
    }

    public class UserChangePasswordDtoValidator : AbstractValidator<UserChangePasswordDto>
    {
        public UserChangePasswordDtoValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Mevcut şifre alanı zorunludur");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Yeni şifre alanı zorunludur")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır")
                .MaximumLength(100).WithMessage("Şifre en fazla 100 karakter olabilir")
                .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir")
                .Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir")
                .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir")
                .Matches("[^a-zA-Z0-9]").WithMessage("Şifre en az bir özel karakter içermelidir");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Şifre tekrar alanı zorunludur")
                .Equal(x => x.NewPassword).WithMessage("Şifreler eşleşmiyor");
        }
    }
} 