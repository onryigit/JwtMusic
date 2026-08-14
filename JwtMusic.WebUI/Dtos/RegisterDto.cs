namespace JwtMusic.WebUI.Dtos
{
    public class RegisterDto
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Kullanıcı adı zorunludur."), System.ComponentModel.DataAnnotations.MinLength(3)]
        public string Username { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Ad zorunludur.")]
        public string Name { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Soyad zorunludur.")]
        public string Surname { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "E-posta zorunludur."), System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = "Geçerli bir e-posta girin.")]
        public string Email { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Parola zorunludur."), System.ComponentModel.DataAnnotations.MinLength(6, ErrorMessage = "Parola en az 6 karakter olmalıdır.")]
        public string Password { get; set; } = string.Empty;
    }
}
