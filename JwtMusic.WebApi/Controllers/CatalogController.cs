using System.Security.Claims;
using JwtMusic.WebApi.Context;
using JwtMusic.WebApi.Dtos;
using JwtMusic.WebApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JwtMusic.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api")]
public class CatalogController : ControllerBase
{
    private readonly JwtContext _context;
    public CatalogController(JwtContext context) => _context = context;

    [HttpGet("artists")]
    public async Task<IReadOnlyCollection<ArtistDto>> Artists()
    {
        var artists = await ArtistQuery().ToListAsync();
        return artists.Select(ToDto).ToList();
    }

    [HttpGet("artists/{id:int}")]
    public async Task<ActionResult<ArtistDto>> Artist(int id)
    {
        var artist = await ArtistQuery().SingleOrDefaultAsync(x => x.ArtistId == id);
        return artist is null ? NotFound() : Ok(ToDto(artist));
    }

    [HttpGet("genres")]
    public async Task<IReadOnlyCollection<GenreDto>> Genres() => await _context.Genres.AsNoTracking()
        .Select(x => new GenreDto(x.GenreId, x.Name, x.Songs.Count)).ToListAsync();

    [HttpGet("albums")]
    public async Task<IReadOnlyCollection<AlbumDto>> Albums() => await _context.Albums.AsNoTracking()
        .Select(x => new AlbumDto(x.AlbumId, x.Name, x.CoverImageUrl, x.ReleaseDate,
            x.ArtistId, x.Artist.ArtistName, x.Songs.Count)).ToListAsync();

    [HttpGet("playlists")]
    public async Task<IReadOnlyCollection<PlaylistDto>> Playlists()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var playlists = await _context.Playlists.AsNoTracking().Where(x => x.AppUserId == userId)
            .Include(x => x.Songs).ThenInclude(x => x.Artist)
            .Include(x => x.Songs).ThenInclude(x => x.Album)
            .Include(x => x.Songs).ThenInclude(x => x.Genre).ToListAsync();
        return playlists.Select(x => new PlaylistDto(x.PlaylistId, x.Name, x.Songs.Select(ToSongDto).ToList())).ToList();
    }

    [HttpPost("playlists")]
    public async Task<ActionResult> CreatePlaylist(CreatePlaylistDto dto)
    {
        var songs = await _context.Songs.Where(x => dto.SongIds.Contains(x.SongId)).ToListAsync();
        var playlist = new Playlist { Name = dto.Name.Trim(), AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!, Songs = songs };
        _context.Playlists.Add(playlist);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Playlists), new { id = playlist.PlaylistId }, playlist.PlaylistId);
    }

    [HttpGet("users/me")]
    public async Task<ActionResult<ProfileDto>> Me()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await _context.Users.AsNoTracking().Where(x => x.Id == id)
            .Select(x => new ProfileDto(x.Id, x.UserName!, x.Name, x.Surname, x.Email!, x.PackageLevel)).SingleAsync();
        return Ok(profile);
    }

    private IQueryable<Artist> ArtistQuery() => _context.Artists.AsNoTracking()
        .Include(x => x.Songs).ThenInclude(x => x.Album)
        .Include(x => x.Songs).ThenInclude(x => x.Genre);

    private static ArtistDto ToDto(Artist x) => new(x.ArtistId, x.ArtistName, x.ArtistImageUrl,
        x.CoverImageUrl, x.Bio, x.Country, x.IsVerified, x.Songs.Select(ToSongDto).ToList());

    private static SongDto ToSongDto(Song x) => new(x.SongId, x.SongName, x.CoverImageUrl, x.Duration,
        x.ListenCount, x.ReleaseDate, x.RequiredPackage, x.Lyrics, x.ArtistId, x.Artist.ArtistName,
        x.AlbumId, x.Album.Name, x.GenreId, x.Genre.Name);
}
