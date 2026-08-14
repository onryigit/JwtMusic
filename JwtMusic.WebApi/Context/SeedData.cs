using JwtMusic.WebApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JwtMusic.WebApi.Context;

public static class SeedData
{
    private static readonly string[] GenreNames = { "Pop", "Rock", "Rap", "Jazz", "Electronic", "R&B", "Classical" };
    private static readonly (string Name, string Country)[] ArtistNames =
    {
        ("Luna Vale", "Türkiye"), ("Neon Harbor", "İngiltere"), ("Atlas Echo", "ABD"),
        ("Mira Blue", "Fransa"), ("The Vinyls", "Kanada"), ("Nova Quartet", "Almanya"),
        ("Karma Flow", "Türkiye")
    };
    private static readonly string[] SongNames =
    {
        "Gece Işıkları", "Kayıp Zaman", "Wild Horizon", "Electric Heart", "Sessiz Şehir",
        "Golden Hour", "Midnight Drive", "Blue Notes", "Runaway Beat", "Ocean Eyes",
        "Echoes", "Velvet Sky", "Son Durak", "Gravity", "Neon Rain", "Dream Sequence",
        "Rüzgar", "Afterglow", "Pulse", "Moonlit Sonata"
    };

    public static async Task InitializeAsync(JwtContext context, UserManager<AppUser> userManager)
    {
        var genres = await context.Genres.ToListAsync();
        foreach (var name in GenreNames.Where(name => genres.All(x => x.Name != name)))
        {
            var genre = new Genre { Name = name };
            genres.Add(genre);
            context.Genres.Add(genre);
        }
        await context.SaveChangesAsync();

        var artists = await context.Artists.ToListAsync();
        for (var i = 0; i < ArtistNames.Length; i++)
        {
            var item = ArtistNames[i];
            if (artists.Any(x => x.ArtistName == item.Name)) continue;
            var artist = new Artist
            {
                ArtistName = item.Name,
                Country = item.Country,
                Bio = $"{item.Name}, modern tınılarla güçlü melodileri buluşturan bağımsız bir sanatçıdır.",
                ArtistImageUrl = $"/Bepop/assets/img/a{i}.jpg",
                CoverImageUrl = $"/Bepop/assets/img/b{i}.jpg",
                CreatedDate = DateTime.UtcNow.AddYears(-3),
                IsVerified = i % 2 == 0
            };
            artists.Add(artist);
            context.Artists.Add(artist);
        }
        await context.SaveChangesAsync();

        var albums = await context.Albums.ToListAsync();
        foreach (var artist in artists)
        {
            if (albums.Any(x => x.ArtistId == artist.ArtistId)) continue;
            var album = new Album
            {
                Name = $"{artist.ArtistName} Sessions",
                ArtistId = artist.ArtistId,
                CoverImageUrl = artist.CoverImageUrl,
                ReleaseDate = new DateTime(2024, 1, 1)
            };
            albums.Add(album);
            context.Albums.Add(album);
        }
        await context.SaveChangesAsync();

        for (var i = 0; i < SongNames.Length; i++)
        {
            if (await context.Songs.AnyAsync(x => x.SongName == SongNames[i])) continue;
            var artist = artists[i % artists.Count];
            context.Songs.Add(new Song
            {
                SongName = SongNames[i],
                ArtistId = artist.ArtistId,
                AlbumId = albums.First(x => x.ArtistId == artist.ArtistId).AlbumId,
                GenreId = genres[i % genres.Count].GenreId,
                RequiredPackage = (PackageLevel)(i % 4),
                CoverImageUrl = $"/Bepop/assets/img/b{i}.jpg",
                AudioUrl = $"track-{i + 1:00}.mp3",
                Duration = TimeSpan.FromSeconds(30),
                ReleaseDate = new DateTime(2023 + i % 4, i % 12 + 1, 1),
                Lyrics = "Bu eser JwtMusic demo kataloğu için hazırlanmıştır."
            });
        }
        await context.SaveChangesAsync();

        foreach (var package in Enum.GetValues<PackageLevel>())
        {
            var username = package.ToString().ToLowerInvariant();
            if (await userManager.FindByNameAsync(username) is not null) continue;
            var user = new AppUser
            {
                UserName = username,
                Email = $"{username}@jwtmusic.local",
                Name = package.ToString(),
                Surname = "Kullanıcı",
                ImageUrl = "/Bepop/assets/img/a0.jpg",
                PackageLevel = package
            };
            var result = await userManager.CreateAsync(user, "Music123");
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
        }
    }
}
