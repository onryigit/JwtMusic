using System.Text.Json;
using System.Text.RegularExpressions;
using JwtMusic.WebApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JwtMusic.WebApi.Context;

public static class SeedData
{
    private static readonly string[] GenreNames = { "Pop", "Rock", "Rap", "Dance", "R&B", "Alternative", "Electronic", "Jazz", "Classical" };

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
        new("Cruel Summer", "Taylor Swift", "Pop", "ABD"),
        new("Kuzu Kuzu", "Tarkan", "Pop", "Türkiye"),
        new("Hadi Bakalım", "Ceylan Ertem", "Pop", "Türkiye"),
        new("Fırtınadayım", "Mabel Matiz", "Pop", "Türkiye"),
        new("Yalnız Çiçek", "Aleyna Tilki", "Pop", "Türkiye"),
        new("Sarı Laleler", "MFÖ", "Rock", "Türkiye"),
        new("Paramparça", "Teoman", "Rock", "Türkiye"),
        new("Cambaz", "Mor ve Ötesi", "Rock", "Türkiye"),
        new("Bu Akşam", "Duman", "Rock", "Türkiye"),
        new("We Could Be the Same", "maNga", "Rock", "Türkiye"),
        new("Holocaust", "Ceza", "Rap", "Türkiye"),
        new("Neyim Var Ki", "Ceza", "Rap", "Türkiye"),
        new("Aya", "Murda & Ezhel", "Rap", "Türkiye"),
        new("Felaket", "Ezhel", "Rap", "Türkiye"),
        new("Bi' Tek Ben Anlarım", "KÖFN", "Electronic", "Türkiye"),
        new("Sensiz Olmaz", "Gripin", "Rock", "Türkiye"),
        new("bad guy", "Billie Eilish", "Alternative", "ABD"),
        new("Starboy", "The Weeknd", "R&B", "Kanada"),
        new("Don't Start Now", "Dua Lipa", "Pop", "Birleşik Krallık"),
        new("Watermelon Sugar", "Harry Styles", "Pop", "Birleşik Krallık"),
        new("Poker Face", "Lady Gaga", "Pop", "ABD"),
        new("HUMBLE.", "Kendrick Lamar", "Rap", "ABD"),
        new("Lose Yourself", "Eminem", "Rap", "ABD"),
        new("Smells Like Teen Spirit", "Nirvana", "Rock", "ABD"),
        new("Sweet Child O' Mine", "Guns N' Roses", "Rock", "ABD"),
        new("Take Five", "The Dave Brubeck Quartet", "Jazz", "ABD"),
        new("So What", "Miles Davis", "Jazz", "ABD"),
        new("Clair de Lune", "Claude Debussy", "Classical", "Fransa"),
        new("Levels", "Avicii", "Electronic", "İsveç"),
        new("Titanium", "David Guetta", "Electronic", "Fransa"),
        new("Get Lucky", "Daft Punk", "Electronic", "Fransa"),
        new("Born to Die", "Lana Del Rey", "Alternative", "ABD", 1440829978),
        new("Video Games", "Lana Del Rey", "Alternative", "ABD", 1440830118),
        new("Summertime Sadness", "Lana Del Rey", "Alternative", "ABD", 1440830261),
        new("Dark Paradise", "Lana Del Rey", "Alternative", "ABD", 1440830128)
    };

    private static readonly FullLengthCatalogItem[] FullLengthCatalog =
    {
        new("A Thousand Years", "33⅓", "Rock", "josh-woodward-01.mp3", "/images/full-tracks/33.jpg", "AThousandYears", 211, new DateTime(2024, 12, 4)),
        new("Circles", "33⅓", "Pop", "josh-woodward-02.mp3", "/images/full-tracks/33.jpg", "Circles", 202, new DateTime(2024, 12, 4)),
        new("Wade", "33⅓", "Pop", "josh-woodward-03.mp3", "/images/full-tracks/33.jpg", "Wade", 185, new DateTime(2024, 12, 4)),
        new("Synthia", "33⅓", "Rock", "josh-woodward-04.mp3", "/images/full-tracks/33.jpg", "Synthia", 175, new DateTime(2024, 12, 5)),
        new("Truly", "33⅓", "Alternative", "josh-woodward-05.mp3", "/images/full-tracks/33.jpg", "Truly", 159, new DateTime(2024, 12, 5)),
        new("33⅓", "33⅓", "Alternative", "josh-woodward-06.mp3", "/images/full-tracks/33.jpg", "33", 170, new DateTime(2024, 12, 5)),
        new("Time for the Past", "33⅓", "Rock", "josh-woodward-07.mp3", "/images/full-tracks/33.jpg", "TimeforthePast", 211, new DateTime(2024, 12, 5)),
        new("Willow, Willow", "33⅓", "Alternative", "josh-woodward-08.mp3", "/images/full-tracks/33.jpg", "WillowWillow", 184, new DateTime(2024, 12, 6)),
        new("Almost No Regrets", "33⅓", "Rock", "josh-woodward-09.mp3", "/images/full-tracks/33.jpg", "AlmostNoRegrets", 265, new DateTime(2024, 12, 6)),
        new("Flotsam", "33⅓", "Rock", "josh-woodward-10.mp3", "/images/full-tracks/33.jpg", "Flotsam", 182, new DateTime(2024, 12, 6)),
        new("I Will Not Let You Let Me Down", "The Wake", "Rock", "josh-woodward-11.mp3", "/images/full-tracks/the-wake.jpg", "IWillNotLetYouLetMeDown", 181, new DateTime(2012, 8, 21)),
        new("This Is Everything", "The Wake", "Rock", "josh-woodward-12.mp3", "/images/full-tracks/the-wake.jpg", "ThisIsEverything", 206, new DateTime(2012, 8, 21)),
        new("Hollow Grove", "The Wake", "Rock", "josh-woodward-13.mp3", "/images/full-tracks/the-wake.jpg", "HollowGrove", 235, new DateTime(2012, 8, 21)),
        new("Crazy Glue", "The Wake", "Alternative", "josh-woodward-14.mp3", "/images/full-tracks/the-wake.jpg", "CrazyGlue", 164, new DateTime(2012, 8, 21)),
        new("Lafayette", "The Wake", "Alternative", "josh-woodward-15.mp3", "/images/full-tracks/the-wake.jpg", "Lafayette", 303, new DateTime(2012, 8, 21)),
        new("Oh Mallory", "The Wake", "Rock", "josh-woodward-16.mp3", "/images/full-tracks/the-wake.jpg", "OhMallory", 196, new DateTime(2012, 8, 21)),
        new("Invisible Light", "The Wake", "Rock", "josh-woodward-17.mp3", "/images/full-tracks/the-wake.jpg", "InvisibleLight", 250, new DateTime(2012, 8, 21)),
        new("Water in the Creek", "The Wake", "Alternative", "josh-woodward-18.mp3", "/images/full-tracks/the-wake.jpg", "WaterintheCreek", 183, new DateTime(2012, 8, 21)),
        new("Little Tomcat", "The Wake", "Alternative", "josh-woodward-19.mp3", "/images/full-tracks/the-wake.jpg", "LittleTomcat", 197, new DateTime(2012, 8, 21)),
        new("Golden Sunrise", "The Wake", "Rock", "josh-woodward-20.mp3", "/images/full-tracks/the-wake.jpg", "GoldenSunrise", 251, new DateTime(2012, 8, 21))
    };

    public static async Task InitializeAsync(JwtContext context, UserManager<AppUser> userManager)
    {
        await EnsureUsersAsync(userManager);
        await EnsureFullLengthCatalogAsync(context);

        var existingSongs = await context.Songs.Include(x => x.Artist).ToListAsync();
        var missingItems = Catalog.Where(item => existingSongs.All(song => !CatalogMatches(song, item))).ToList();
        if (missingItems.Count == 0) return;

        var resolved = await ResolveCatalogAsync(missingItems);
        if (resolved.Count != missingItems.Count)
            throw new InvalidOperationException(
                $"Eksik {missingItems.Count}  şarkılardan yalnızca {resolved.Count} tanesi bulunabildi. " +
                "Katalog değiştirilmedi; internet bağlantınızı kontrol edip API'yi yeniden başlatın.");

        await using var transaction = await context.Database.BeginTransactionAsync();

        var genres = await context.Genres.ToListAsync();
        foreach (var name in GenreNames.Where(name => genres.All(x => x.Name != name)))
        {
            var genre = new Genre { Name = name };
            genres.Add(genre);
            context.Genres.Add(genre);
        }
        await context.SaveChangesAsync();

        var artists = (await context.Artists.ToListAsync()).GroupBy(x => x.ArtistName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);
        var albums = (await context.Albums.Include(x => x.Artist).ToListAsync())
            .GroupBy(x => $"{x.Artist.ArtistName}|{x.Name}", StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);
        foreach (var resolvedItem in resolved)
        {
            var source = resolvedItem.Source;
            var track = resolvedItem.Track;
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
                RequiredTier = (MembershipTier)(Array.IndexOf(Catalog, source) % 4 + 1),
                CoverImageUrl = track.ArtworkUrl,
                AudioUrl = track.PreviewUrl,
                StoreUrl = track.TrackViewUrl,
                Duration = TimeSpan.FromSeconds(30),
                ReleaseDate = track.ReleaseDate,
                Lyrics = string.Empty
            });
        }

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private static async Task EnsureFullLengthCatalogAsync(JwtContext context)
    {
        var genres = await context.Genres.ToListAsync();
        foreach (var name in FullLengthCatalog.Select(x => x.Genre).Distinct()
                     .Where(name => genres.All(x => x.Name != name)))
        {
            var genre = new Genre { Name = name };
            genres.Add(genre);
            context.Genres.Add(genre);
        }

        var artist = await context.Artists.Include(x => x.Songs)
            .SingleOrDefaultAsync(x => x.ArtistName == "Josh Woodward");
        if (artist is null)
        {
            artist = new Artist
            {
                ArtistName = "Josh Woodward",
                Country = "ABD",
                Bio = "Creative Commons lisanslı bağımsız müzik sanatçısı.",
                ArtistImageUrl = "/images/full-tracks/33.jpg",
                CoverImageUrl = "/images/full-tracks/the-wake.jpg",
                CreatedDate = DateTime.UtcNow,
                IsVerified = true
            };
            context.Artists.Add(artist);
        }

        var albums = await context.Albums.Include(x => x.Artist)
            .Where(x => x.Artist.ArtistName == "Josh Woodward")
            .ToDictionaryAsync(x => x.Name);
        foreach (var source in FullLengthCatalog)
        {
            if (!albums.TryGetValue(source.Album, out var album))
            {
                album = new Album
                {
                    Name = source.Album,
                    Artist = artist,
                    CoverImageUrl = source.CoverImageUrl,
                    ReleaseDate = source.ReleaseDate
                };
                albums.Add(source.Album, album);
                context.Albums.Add(album);
            }

            if (artist.Songs.Any(x => Normalize(x.SongName) == Normalize(source.Title))) continue;
            var song = new Song
            {
                SongName = source.Title,
                Artist = artist,
                Album = album,
                Genre = genres.First(x => x.Name == source.Genre),
                RequiredTier = MembershipTier.Basic,
                CoverImageUrl = source.CoverImageUrl,
                AudioUrl = source.AudioFileName,
                StoreUrl = $"https://www.joshwoodward.com/song/{source.SourceSlug}",
                Duration = TimeSpan.FromSeconds(source.DurationSeconds),
                ReleaseDate = source.ReleaseDate,
                Lyrics = string.Empty
            };
            artist.Songs.Add(song);
            context.Songs.Add(song);
        }

        await context.SaveChangesAsync();
    }

    private static async Task<List<ResolvedCatalogItem>> ResolveCatalogAsync(IReadOnlyCollection<CatalogItem> items)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("JwtMusic/1.0");
        var results = new List<ResolvedCatalogItem>();
        foreach (var item in items)
        {
            ItunesTrack? track = null;
            for (var attempt = 1; attempt <= 4 && track is null; attempt++)
            {
                var term = Uri.EscapeDataString($"{item.Artist} {item.Title}");
                var url = item.StoreId.HasValue
                    ? $"https://itunes.apple.com/lookup?id={item.StoreId.Value}&country=tr&entity=song"
                    : $"https://itunes.apple.com/search?term={term}&country=tr&media=music&entity=song&limit=10";
                using var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var payload = JsonSerializer.Deserialize<ItunesResponse>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                    track = payload?.Results
                        .Where(x => !string.IsNullOrWhiteSpace(x.PreviewUrl) && !string.IsNullOrWhiteSpace(x.ArtworkUrl100))
                        .Where(x => TrackMatches(x, item))
                        .OrderByDescending(x => MatchScore(x, item))
                        .FirstOrDefault();
                }
                else if ((int)response.StatusCode is 403 or 429 && attempt < 4)
                {
                    await Task.Delay(TimeSpan.FromSeconds(attempt * 10));
                }
                else
                {
                    response.EnsureSuccessStatusCode();
                }


                await Task.Delay(TimeSpan.FromMilliseconds(3100));
            }

            if (track is null) continue;
            track.ArtworkUrl100 = Regex.Replace(track.ArtworkUrl100, @"\d+x\d+bb", "600x600bb");
            results.Add(new ResolvedCatalogItem(item, track));
        }

        return results;
    }

    private static bool CatalogMatches(Song song, CatalogItem item)
    {
        var title = Normalize(song.SongName);
        var artist = Normalize(song.Artist.ArtistName);
        var wantedTitle = Normalize(item.Title);
        var wantedArtist = Normalize(item.Artist);
        return (title == wantedTitle || title.Contains(wantedTitle) || wantedTitle.Contains(title)) &&
               (artist == wantedArtist || artist.Contains(wantedArtist) || wantedArtist.Contains(artist));
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

    private static bool TrackMatches(ItunesTrack track, CatalogItem item)
    {
        var title = Normalize(track.TrackName);
        var artist = Normalize(track.ArtistName);
        var wantedTitle = Normalize(item.Title);
        var wantedArtist = Normalize(item.Artist);
        return (title == wantedTitle || title.Contains(wantedTitle) || wantedTitle.Contains(title)) &&
               (artist == wantedArtist || artist.Contains(wantedArtist) || wantedArtist.Contains(artist));
    }

    private static string Normalize(string value) => new(value.ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());

    private static async Task EnsureUsersAsync(UserManager<AppUser> userManager)
    {
        foreach (var package in Enum.GetValues<MembershipTier>())
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
                PlanTier = package
            };
            var result = await userManager.CreateAsync(user, "Music123");
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
        }
    }

    private sealed record CatalogItem(string Title, string Artist, string Genre, string Country, long? StoreId = null);
    private sealed record FullLengthCatalogItem(string Title, string Album, string Genre, string AudioFileName,
        string CoverImageUrl, string SourceSlug, int DurationSeconds, DateTime ReleaseDate);
    private sealed record ResolvedCatalogItem(CatalogItem Source, ItunesTrack Track);
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
