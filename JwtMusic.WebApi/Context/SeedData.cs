using JwtMusic.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtMusic.WebApi.Context;

public static class SeedData
{
    private static readonly string[] GenreNames = { "Pop", "Rock", "Rap", "Jazz", "Electronic", "R&B", "Classical" };
    private static readonly (string Artist, string Country)[] ArtistNames =
    {
        ("Luna Vale", "Türkiye"), ("Neon Harbor", "İngiltere"), ("Atlas Echo", "ABD"),
        ("Mira Blue", "Fransa"), ("The Vinyls", "Kanada"), ("Nova Quartet", "Almanya"), ("Karma Flow", "Türkiye")
    };
    private static readonly string[] SongNames =
    {
        "Gece Işıkları", "Kayıp Zaman", "Wild Horizon", "Electric Heart", "Sessiz Şehir",
        "Golden Hour", "Midnight Drive", "Blue Notes", "Runaway Beat", "Ocean Eyes",
        "Echoes", "Velvet Sky", "Son Durak", "Gravity", "Neon Rain",
        "Dream Sequence", "Rüzgar", "Afterglow", "Pulse", "Moonlit Sonata"
    };

    public static async Task InitializeAsync(JwtContext context)
    {
        var genres = await context.Genres.ToListAsync();
        var missingGenres = GenreNames.Except(genres.Select(x => x.Name)).Select(x => new Genre { Name = x }).ToArray();
        if (missingGenres.Length > 0) { context.AddRange(missingGenres); await context.SaveChangesAsync(); genres.AddRange(missingGenres); }

        var artists = await context.Artists.ToListAsync();
        var missingArtists = ArtistNames.Where(x => artists.All(a => a.ArtistName != x.Artist))
            .Select((x, i) => new Artist
            {
                ArtistName = x.Artist, Country = x.Country, Bio = $"{x.Artist} müziğin sınırlarını keşfeden bağımsız bir sanatçıdır.",
                ArtistImageUrl = $"/Bepop/assets/img/a{i}.jpg", CoverImageUrl = $"/Bepop/assets/img/b{i}.jpg",
                CreatedDate = DateTime.UtcNow.AddYears(-3), IsVerified = i % 2 == 0
            }).ToList();
        if (missingArtists.Count > 0) { context.AddRange(missingArtists); await context.SaveChangesAsync(); artists.AddRange(missingArtists); }

        var albums = await context.Albums.ToListAsync();
        foreach (var artist in artists.Where(x => albums.All(a => a.ArtistId != x.ArtistId)))
            albums.Add(new Album { Name = $"{artist.ArtistName} Sessions", ArtistId = artist.ArtistId,
                CoverImageUrl = artist.CoverImageUrl, ReleaseDate = new DateTime(2024, 1, 1) });
        if (context.ChangeTracker.HasChanges()) await context.SaveChangesAsync();

        var demoSongs = await context.Songs.Where(x => SongNames.Contains(x.SongName)).ToListAsync();
        for (var i = 0; i < demoSongs.Count; i++)
        {
            var artist = artists[i % artists.Count];
            demoSongs[i].ArtistId = artist.ArtistId;
            demoSongs[i].AlbumId = albums.First(x => x.ArtistId == artist.ArtistId).AlbumId;
        }
        if (context.ChangeTracker.HasChanges()) await context.SaveChangesAsync();

        var existingCount = await context.Songs.CountAsync();
        if (existingCount >= 20) return;
        var songs = SongNames.Skip(existingCount).Select((name, offset) =>
        {
            var i = existingCount + offset;
            var artist = artists[i % artists.Count];
            var album = albums.First(x => x.ArtistId == artist.ArtistId);
            return new Song
            {
            SongName = name, ArtistId = artist.ArtistId, AlbumId = album.AlbumId,
            GenreId = genres[i % genres.Count].GenreId, RequiredPackage = (PackageLevel)(i % 4),
            CoverImageUrl = $"/Bepop/assets/img/b{i}.jpg", AudioUrl = $"track-{i + 1:00}.mp3",
            Duration = TimeSpan.FromSeconds(30), ReleaseDate = new DateTime(2023 + i % 4, i % 12 + 1, 1),
            Lyrics = "Bu eser JwtMusic demo kataloğu için hazırlanmıştır."
            };
        });
        context.AddRange(songs); await context.SaveChangesAsync();
    }
}
