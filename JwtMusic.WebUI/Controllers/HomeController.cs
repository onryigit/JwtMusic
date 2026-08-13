using JwtMusic.WebUI.Models;
using JwtMusic.WebUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusic.WebUI.Controllers;

public class HomeController : Controller
{
    private readonly MusicApiClient _api;
    public HomeController(MusicApiClient api) => _api = api;

    public async Task<IActionResult> Index(int? genreId)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"))) return RedirectToAction("SignIn", "Login");
        var songs = await _api.GetAsync<List<SongViewModel>>($"api/songs{(genreId.HasValue ? $"?genreId={genreId}" : "")}");
        if (songs is null) return RedirectToAction("SignIn", "Login");
        var genres = await _api.GetAsync<List<GenreViewModel>>("api/genres") ?? new();
        return View(new HomeViewModel(songs, genres, genreId));
    }
}
