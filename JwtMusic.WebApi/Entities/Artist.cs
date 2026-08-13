namespace JwtMusic.WebApi.Entities
{
    public class Artist
    {
        public int ArtistId { get; set; }

        public string ArtistName { get; set; } = string.Empty;

        public string ArtistImageUrl { get; set; } = string.Empty;

        public string CoverImageUrl { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public bool IsVerified { get; set; }

        public List<Song> Songs { get; set; } = new();
    }
}
