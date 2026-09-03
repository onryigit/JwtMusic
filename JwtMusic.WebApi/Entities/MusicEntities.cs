namespace JwtMusic.WebApi.Entities;

public enum MembershipTier
{
    Basic = 1,
    Gold = 2,
    Premium = 3,
    Elit = 4
}

public class Genre
{
    public int GenreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Song> Songs { get; set; } = new();
}

public class Album
{
    public int AlbumId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public int ArtistId { get; set; }
    public Artist Artist { get; set; } = null!;
    public List<Song> Songs { get; set; } = new();
}

public class Playlist
{
    public int PlaylistId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AppUserId { get; set; } = string.Empty;
    public AppUser AppUser { get; set; } = null!;
    public List<Song> Songs { get; set; } = new();
}

public class ListeningHistory
{
    public long ListeningHistoryId { get; set; }
    public string AppUserId { get; set; } = string.Empty;
    public AppUser AppUser { get; set; } = null!;
    public int SongId { get; set; }
    public Song Song { get; set; } = null!;
    public DateTime ListenedAt { get; set; }
}
