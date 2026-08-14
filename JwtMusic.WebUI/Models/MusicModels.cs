namespace JwtMusic.WebUI.Models;

public record SongViewModel(int SongId, string SongName, string CoverImageUrl, string StoreUrl, TimeSpan Duration,
    int ListenCount, DateTime ReleaseDate, string RequiredPackage, string Lyrics,
    int ArtistId, string ArtistName, int AlbumId, string AlbumName, int GenreId, string GenreName);
public record ArtistViewModel(int ArtistId, string ArtistName, string ArtistImageUrl, string CoverImageUrl,
    string Bio, string Country, bool IsVerified, IReadOnlyCollection<SongViewModel> Songs);
public record GenreViewModel(int GenreId, string Name, int SongCount);
public record HomeViewModel(IReadOnlyCollection<SongViewModel> Songs, IReadOnlyCollection<GenreViewModel> Genres, int? SelectedGenreId);
public record SongDetailViewModel(SongViewModel Song, IReadOnlyCollection<SongViewModel> Recommendations);
public record AlbumViewModel(int AlbumId, string Name, string CoverImageUrl, DateTime ReleaseDate,
    int ArtistId, string ArtistName, int SongCount);
public record HistoryViewModel(long ListeningHistoryId, DateTime ListenedAt, SongViewModel Song);
public record PlaylistViewModel(int PlaylistId, string Name, IReadOnlyCollection<SongViewModel> Songs);
public record LibraryViewModel(IReadOnlyCollection<PlaylistViewModel> Playlists, IReadOnlyCollection<SongViewModel> Songs);
