namespace JwtMusic.WebUI.Dtos
{
    public class LoginDto
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Kullanıcı adı veya e-posta zorunludur.")]
        public string Username { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Parola zorunludur.")]
        public string Password { get; set; } = string.Empty;
    }
}
