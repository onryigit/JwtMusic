using JwtMusic.WebUI.Models;
using JwtMusic.WebUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusic.WebUI.Controllers;

public class LibraryController : Controller
{
    private readonly MusicApiClient _api;
    public LibraryController(MusicApiClient api) => _api = api;

    public async Task<IActionResult> Genres()
    {
        if (!IsSignedIn()) return SignIn();
        var genres = await _api.GetAsync<List<GenreViewModel>>("api/genres");
        return genres is null ? SignIn() : View(genres);
    }

    public async Task<IActionResult> Albums()
    {
        if (!IsSignedIn()) return SignIn();
        var albums = await _api.GetAsync<List<AlbumViewModel>>("api/albums");
        return albums is null ? SignIn() : View(albums);
    }

    public async Task<IActionResult> History()
    {
        if (!IsSignedIn()) return SignIn();
        var history = await _api.GetAsync<List<HistoryViewModel>>("api/users/me/history");
        return history is null ? SignIn() : View(history);
    }

    public async Task<IActionResult> Playlists()
    {
        if (!IsSignedIn()) return SignIn();
        var playlists = await _api.GetAsync<List<PlaylistViewModel>>("api/playlists");
        var songs = await _api.GetAsync<List<SongViewModel>>("api/songs");
        return playlists is null || songs is null ? SignIn() : View(new LibraryViewModel(playlists, songs));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePlaylist(string name, int[] songIds)
    {
        if (!IsSignedIn()) return SignIn();
        using var response = await _api.PostAsync("api/playlists", new { name, songIds });
        TempData[response.IsSuccessStatusCode ? "Success" : "Error"] = response.IsSuccessStatusCode
            ? "Playlist oluşturuldu." : "Playlist oluşturulamadı.";
        return RedirectToAction(nameof(Playlists));
    }

    private bool IsSignedIn() => !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("JwtToken"));
    private RedirectToActionResult SignIn() => RedirectToAction("SignIn", "Login");
}
