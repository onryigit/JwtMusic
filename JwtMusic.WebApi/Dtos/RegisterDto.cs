namespace JwtMusic.WebApi.Dtos
{
    public class RegisterDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string Name { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required]
        public string Surname { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.EmailAddress]
        public string Email { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.MinLength(3)]
        public string Username { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.MinLength(6)]
        public string Password { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.EnumDataType(typeof(Entities.MembershipTier))]
        public Entities.MembershipTier PlanTier { get; set; } = Entities.MembershipTier.Basic;
    }
}
