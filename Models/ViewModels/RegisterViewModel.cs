using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesi.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre onayı gereklidir.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifre ve onay şifresi eşleşmiyor.")]
        public string ConfirmPassword { get; set; }
    }
}
