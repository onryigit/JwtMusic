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
        if (await context.Songs.AnyAsync()) return;
        var genres = GenreNames.Select(x => new Genre { Name = x }).ToArray();
        var artists = ArtistNames.Select((x, i) => new Artist
        {
            ArtistName = x.Artist, Country = x.Country, Bio = $"{x.Artist} müziğin sınırlarını keşfeden bağımsız bir sanatçıdır.",
            ArtistImageUrl = $"/Bepop/assets/img/a{i}.jpg", CoverImageUrl = $"/Bepop/assets/img/b{i}.jpg",
            CreatedDate = DateTime.UtcNow.AddYears(-3), IsVerified = i % 2 == 0
        }).ToArray();
        context.AddRange(genres); context.AddRange(artists); await context.SaveChangesAsync();
        var albums = artists.Select((x, i) => new Album
        {
            Name = $"{x.ArtistName} Sessions", ArtistId = x.ArtistId,
            CoverImageUrl = $"/Bepop/assets/img/b{i}.jpg", ReleaseDate = new DateTime(2024 + i % 3, i % 12 + 1, 1)
        }).ToArray();
        context.AddRange(albums); await context.SaveChangesAsync();
        var songs = SongNames.Select((name, i) => new Song
        {
            SongName = name, ArtistId = artists[i % artists.Length].ArtistId, AlbumId = albums[i % albums.Length].AlbumId,
            GenreId = genres[i % genres.Length].GenreId, RequiredPackage = (PackageLevel)(i % 4),
            CoverImageUrl = $"/Bepop/assets/img/b{i}.jpg", AudioUrl = $"track-{i + 1:00}.mp3",
            Duration = TimeSpan.FromSeconds(30), ReleaseDate = new DateTime(2023 + i % 4, i % 12 + 1, 1),
            Lyrics = "Bu eser JwtMusic demo kataloğu için hazırlanmıştır."
        });
        context.AddRange(songs); await context.SaveChangesAsync();
    }
}
