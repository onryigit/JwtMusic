using Microsoft.AspNetCore.Identity;

namespace JwtMusic.WebApi.Entities
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column("PackageLevel")]
        public MembershipTier PlanTier { get; set; } = MembershipTier.Basic;
        public List<ListeningHistory> ListeningHistory { get; set; } = new();
    }
}
