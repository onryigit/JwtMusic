using JwtMusic.WebApi.Entities;

namespace JwtMusic.WebApi.Dtos;

public record SongDto(int SongId, string SongName, string CoverImageUrl, string StoreUrl, TimeSpan Duration,
    int ListenCount, DateTime ReleaseDate, MembershipTier RequiredTier, string Lyrics,
    int ArtistId, string ArtistName, int AlbumId, string AlbumName, int GenreId, string GenreName);
public record ArtistDto(int ArtistId, string ArtistName, string ArtistImageUrl, string CoverImageUrl,
    string Bio, string Country, bool IsVerified, IReadOnlyCollection<SongDto> Songs);
public record GenreDto(int GenreId, string Name, int SongCount);
public record AlbumDto(int AlbumId, string Name, string CoverImageUrl, DateTime ReleaseDate,
    int ArtistId, string ArtistName, int SongCount);
public record CreatePlaylistDto(string Name, int[] SongIds);
public record PlaylistDto(int PlaylistId, string Name, IReadOnlyCollection<SongDto> Songs);
public record ProfileDto(string Id, string Username, string Name, string Surname, string Email, MembershipTier PlanTier);
public record HistoryDto(long ListeningHistoryId, DateTime ListenedAt, SongDto Song);

public record UpgradeSubscriptionDto(MembershipTier NewTier);
