using System.ComponentModel.DataAnnotations;

namespace FreelancerCRM.API.DTOs
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Ad alanı zorunludur")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı zorunludur")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı zorunludur")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6, en fazla 100 karakter olmalıdır")]
        public string Password { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "Telefon numarası en fazla 15 karakter olabilir")]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UserUpdateDto
    {
        [Required(ErrorMessage = "Ad alanı zorunludur")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı zorunludur")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir")]
        public string Email { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "Telefon numarası en fazla 15 karakter olabilir")]
        public string? PhoneNumber { get; set; }

        [StringLength(11, MinimumLength = 11, ErrorMessage = "TCKN 11 haneli olmalıdır")]
        public string? TCKN { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Vergi numarası 10 haneli olmalıdır")]
        public string? TaxNumber { get; set; }

        [StringLength(100, ErrorMessage = "Vergi dairesi en fazla 100 karakter olabilir")]
        public string? TaxOffice { get; set; }

        public bool IsKDVMukellefi { get; set; }

        [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir")]
        public string? Address { get; set; }

        public bool IsActive { get; set; }
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? TCKN { get; set; }
        public string? TaxNumber { get; set; }
        public string? TaxOffice { get; set; }
        public bool IsKDVMukellefi { get; set; }
        public string? Address { get; set; }
        public bool KVKKConsent { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UserSummaryDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserLoginDto
    {
        [Required(ErrorMessage = "E-posta alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı zorunludur")]
        public string Password { get; set; } = string.Empty;
    }

    public class UserChangePasswordDto
    {
        [Required(ErrorMessage = "Kullanıcı ID zorunludur")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Mevcut şifre alanı zorunludur")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yeni şifre alanı zorunludur")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6, en fazla 100 karakter olmalıdır")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrar alanı zorunludur")]
        [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
} 