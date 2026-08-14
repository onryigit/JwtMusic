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
[Route("api/[controller]")]
public class SongsController : ControllerBase
{
    private readonly JwtContext _context;
    private readonly IWebHostEnvironment _environment;
    public SongsController(JwtContext context, IWebHostEnvironment environment) =>
        (_context, _environment) = (context, environment);

    [HttpGet]
    public async Task<IReadOnlyCollection<SongDto>> GetAll([FromQuery] int? artistId, [FromQuery] int? genreId)
    {
        var query = _context.Songs.AsNoTracking().AsQueryable();
        if (artistId.HasValue) query = query.Where(x => x.ArtistId == artistId);
        if (genreId.HasValue) query = query.Where(x => x.GenreId == genreId);
        return await Project(query).ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> Get(int id)
    {
        var song = await Project(_context.Songs.AsNoTracking().Where(x => x.SongId == id)).SingleOrDefaultAsync();
        if (song is null) return NotFound();
        var listenerIds = _context.ListeningHistory.Where(x => x.SongId == id).Select(x => x.AppUserId);
        var suggestedIds = await _context.ListeningHistory
            .Where(x => x.SongId != id && listenerIds.Contains(x.AppUserId))
            .GroupBy(x => x.SongId).OrderByDescending(x => x.Count()).Select(x => x.Key).Take(6).ToListAsync();
        var recommendations = await Project(_context.Songs.AsNoTracking()
            .Where(x => x.SongId != id && (suggestedIds.Contains(x.SongId) || x.GenreId == song.GenreId)))
            .OrderByDescending(x => suggestedIds.Contains(x.SongId)).ThenByDescending(x => x.ListenCount)
            .Take(6).ToListAsync();
        return Ok(new { song, recommendations });
    }

    [HttpGet("{id:int}/stream")]
    public async Task<IActionResult> Stream(int id)
    {
        var song = await _context.Songs.FindAsync(id);
        if (song is null) return NotFound();
        var package = Enum.TryParse<PackageLevel>(User.FindFirstValue("package"), out var value) ? value : PackageLevel.Basic;
        if (package < song.RequiredPackage)
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Mevcut paketiniz bu şarkıyı desteklememektedir. Lütfen paketinizi yükseltin." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        _context.ListeningHistory.Add(new ListeningHistory { AppUserId = userId, SongId = id, ListenedAt = DateTime.UtcNow });
        song.ListenCount++;
        await _context.SaveChangesAsync();
        var path = Path.Combine(_environment.ContentRootPath, "Audio", Path.GetFileName(song.AudioUrl));
        return System.IO.File.Exists(path) ? PhysicalFile(path, "audio/mpeg", enableRangeProcessing: true) : NotFound();
    }

    internal static IQueryable<SongDto> Project(IQueryable<Song> query) => query.Select(x => new SongDto(
        x.SongId, x.SongName, x.CoverImageUrl, x.Duration, x.ListenCount, x.ReleaseDate, x.RequiredPackage,
        x.Lyrics, x.ArtistId, x.Artist.ArtistName, x.AlbumId, x.Album.Name, x.GenreId, x.Genre.Name));
}
