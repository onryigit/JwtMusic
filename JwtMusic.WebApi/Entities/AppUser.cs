using Microsoft.AspNetCore.Identity;

namespace JwtMusic.WebApi.Entities
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public PackageLevel PackageLevel { get; set; } = PackageLevel.Basic;
        public List<ListeningHistory> ListeningHistory { get; set; } = new();
    }
}
