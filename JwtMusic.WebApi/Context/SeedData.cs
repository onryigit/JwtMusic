using System.Text.Json;
using System.Text.RegularExpressions;
using JwtMusic.WebApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JwtMusic.WebApi.Context;

public static class SeedData
{
    private static readonly string[] GenreNames = { "Pop", "Rock", "Rap", "Dance", "R&B", "Alternative", "Electronic" };

    private static readonly CatalogItem[] Catalog =
    {
        new("Şımarık", "Tarkan", "Pop", "Türkiye"),
        new("Gülümse", "Sezen Aksu", "Pop", "Türkiye"),
        new("Antidepresan", "Mabel Matiz", "Pop", "Türkiye"),
        new("Aşkın Olayım", "Simge", "Pop", "Türkiye"),
        new("Canın Sağ Olsun", "Semicenk", "Pop", "Türkiye"),
        new("Ara", "Zeynep Bastık", "Pop", "Türkiye"),
        new("Martılar", "Edis", "Pop", "Türkiye"),
        new("Düm Tek Tek", "Hadise", "Pop", "Türkiye"),
        new("Çakkıdı", "Kenan Doğulu", "Pop", "Türkiye"),
        new("Everyway That I Can", "Sertab Erener", "Pop", "Türkiye"),
        new("Bir Derdim Var", "Mor ve Ötesi", "Rock", "Türkiye"),
        new("Senden Daha Güzel", "Duman", "Rock", "Türkiye"),
        new("Dünyanın Sonuna Doğmuşum", "maNga", "Rock", "Türkiye"),
        new("For Real", "Athena", "Rock", "Türkiye"),
        new("Geceler", "Ezhel", "Rap", "Türkiye", 1308387097),
        new("Blinding Lights", "The Weeknd", "Pop", "Kanada"),
        new("BIRDS OF A FEATHER", "Billie Eilish", "Alternative", "ABD"),
        new("Levitating", "Dua Lipa", "Pop", "Birleşik Krallık"),
        new("As It Was", "Harry Styles", "Pop", "Birleşik Krallık"),
        new("Die With A Smile", "Lady Gaga & Bruno Mars", "Pop", "ABD"),
        new("Espresso", "Sabrina Carpenter", "Pop", "ABD"),
        new("Shape of You", "Ed Sheeran", "Pop", "Birleşik Krallık"),
        new("Rolling in the Deep", "Adele", "Pop", "Birleşik Krallık"),
        new("Viva La Vida", "Coldplay", "Alternative", "Birleşik Krallık"),
        new("Believer", "Imagine Dragons", "Rock", "ABD"),
        new("Do I Wanna Know?", "Arctic Monkeys", "Rock", "Birleşik Krallık"),
        new("Diamonds", "Rihanna", "R&B", "Barbados"),
        new("Locked Out of Heaven", "Bruno Mars", "Pop", "ABD"),
        new("Flowers", "Miley Cyrus", "Pop", "ABD"),
        new("Cruel Summer", "Taylor Swift", "Pop", "ABD")
    };

    public static async Task InitializeAsync(JwtContext context, UserManager<AppUser> userManager)
    {
        await EnsureUsersAsync(userManager);

        if (await context.Songs.CountAsync() == Catalog.Length &&
            await context.Songs.AllAsync(x => x.AudioUrl.StartsWith("https://") && x.StoreUrl.StartsWith("https://")) &&
            await context.Songs.AnyAsync(x => x.SongName == "Geceler" && x.Artist.ArtistName == "Ezhel"))
            return;

        var resolved = await ResolveCatalogAsync();
        if (resolved.Count != Catalog.Length)
            throw new InvalidOperationException("30 parçalık resmi önizleme kataloğu hazırlanamadı. İnternet bağlantınızı kontrol edip API'yi yeniden başlatın.");

        await using var transaction = await context.Database.BeginTransactionAsync();
        await context.Database.ExecuteSqlRawAsync("DELETE FROM PlaylistSong");
        await context.ListeningHistory.ExecuteDeleteAsync();
        await context.Songs.ExecuteDeleteAsync();
        await context.Albums.ExecuteDeleteAsync();
        await context.Artists.ExecuteDeleteAsync();

        var genres = await context.Genres.ToListAsync();
        foreach (var name in GenreNames.Where(name => genres.All(x => x.Name != name)))
        {
            var genre = new Genre { Name = name };
            genres.Add(genre);
            context.Genres.Add(genre);
        }
        await context.SaveChangesAsync();

        var artists = new Dictionary<string, Artist>(StringComparer.OrdinalIgnoreCase);
        var albums = new Dictionary<string, Album>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < resolved.Count; i++)
        {
            var source = Catalog[i];
            var track = resolved[i];
            if (!artists.TryGetValue(track.ArtistName, out var artist))
            {
                artist = new Artist
                {
                    ArtistName = track.ArtistName,
                    Country = source.Country,
                    Bio = $"{track.ArtistName} kataloğundaki seçili parçaları dinleyin.",
                    ArtistImageUrl = track.ArtworkUrl,
                    CoverImageUrl = track.ArtworkUrl,
                    CreatedDate = DateTime.UtcNow,
                    IsVerified = true
                };
                artists.Add(track.ArtistName, artist);
                context.Artists.Add(artist);
            }

            var albumKey = $"{track.ArtistName}|{track.CollectionName}";
            if (!albums.TryGetValue(albumKey, out var album))
            {
                album = new Album
                {
                    Name = track.CollectionName,
                    Artist = artist,
                    CoverImageUrl = track.ArtworkUrl,
                    ReleaseDate = track.ReleaseDate
                };
                albums.Add(albumKey, album);
                context.Albums.Add(album);
            }

            context.Songs.Add(new Song
            {
                SongName = track.TrackName,
                Artist = artist,
                Album = album,
                Genre = genres.First(x => x.Name == source.Genre),
                RequiredPackage = (PackageLevel)(i % 4),
                CoverImageUrl = track.ArtworkUrl,
                AudioUrl = track.PreviewUrl,
                StoreUrl = track.TrackViewUrl,
                Duration = TimeSpan.FromSeconds(30),
                ReleaseDate = track.ReleaseDate,
                Lyrics = "Bu sayfada eserin resmi 30 saniyelik önizlemesi sunulmaktadır."
            });
        }

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private static async Task<List<ItunesTrack>> ResolveCatalogAsync()
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("JwtMusic/1.0");
        var results = new ItunesTrack?[Catalog.Length];
        using var gate = new SemaphoreSlim(4);
        await Task.WhenAll(Catalog.Select(async (item, index) =>
        {
            await gate.WaitAsync();
            try
            {
                var term = Uri.EscapeDataString($"{item.Artist} {item.Title}");
                var url = item.StoreId.HasValue
                    ? $"https://itunes.apple.com/lookup?id={item.StoreId.Value}&country=tr&entity=song"
                    : $"https://itunes.apple.com/search?term={term}&country=tr&media=music&entity=song&limit=10";
                var json = await client.GetStringAsync(url);
                var response = JsonSerializer.Deserialize<ItunesResponse>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                results[index] = response?.Results
                    .Where(x => !string.IsNullOrWhiteSpace(x.PreviewUrl) && !string.IsNullOrWhiteSpace(x.ArtworkUrl100))
                    .OrderByDescending(x => MatchScore(x, item))
                    .FirstOrDefault();
            }
            finally { gate.Release(); }
        }));

        return results.Where(x => x is not null).Select(x =>
        {
            x!.ArtworkUrl100 = Regex.Replace(x.ArtworkUrl100, @"\d+x\d+bb", "600x600bb");
            return x;
        }).ToList();
    }

    private static int MatchScore(ItunesTrack track, CatalogItem item)
    {
        var title = Normalize(track.TrackName);
        var artist = Normalize(track.ArtistName);
        var wantedTitle = Normalize(item.Title);
        var wantedArtist = Normalize(item.Artist);
        return (title == wantedTitle ? 8 : title.Contains(wantedTitle) ? 4 : 0) +
               (artist == wantedArtist ? 6 : artist.Contains(wantedArtist) || wantedArtist.Contains(artist) ? 3 : 0);
    }

    private static string Normalize(string value) => new(value.ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());

    private static async Task EnsureUsersAsync(UserManager<AppUser> userManager)
    {
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
                ImageUrl = string.Empty,
                PackageLevel = package
            };
            var result = await userManager.CreateAsync(user, "Music123");
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
        }
    }

    private sealed record CatalogItem(string Title, string Artist, string Genre, string Country, long? StoreId = null);
    private sealed class ItunesResponse { public List<ItunesTrack> Results { get; set; } = new(); }
    private sealed class ItunesTrack
    {
        public string TrackName { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public string CollectionName { get; set; } = "Single";
        public string PreviewUrl { get; set; } = string.Empty;
        public string TrackViewUrl { get; set; } = string.Empty;
        public string ArtworkUrl100 { get; set; } = string.Empty;
        public string ArtworkUrl { get => ArtworkUrl100; }
        public DateTime ReleaseDate { get; set; }
    }
}
