namespace JwtMusic.WebApi.Entities
{
    public class Song
    {
        public int SongId { get; set; }

        public string SongName { get; set; } = string.Empty;

        public string CoverImageUrl { get; set; } = string.Empty;

        public string AudioUrl { get; set; } = string.Empty;

        public TimeSpan Duration { get; set; }

        public int ListenCount { get; set; }

        public DateTime ReleaseDate { get; set; }

        public PackageLevel RequiredPackage { get; set; }
        public string Lyrics { get; set; } = string.Empty;

        public int ArtistId { get; set; }

        public Artist Artist { get; set; } = null!;
        public int AlbumId { get; set; }
        public Album Album { get; set; } = null!;
        public int GenreId { get; set; }
        public Genre Genre { get; set; } = null!;
        public List<Playlist> Playlists { get; set; } = new();
        public List<ListeningHistory> ListeningHistory { get; set; } = new();
    }
}
