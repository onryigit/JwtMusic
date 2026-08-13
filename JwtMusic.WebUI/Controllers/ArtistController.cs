using JwtMusic.WebUI.Models;
using JwtMusic.WebUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusic.WebUI.Controllers;

public class ArtistController : Controller
{
    private readonly MusicApiClient _api;
    public ArtistController(MusicApiClient api) => _api = api;

    public async Task<IActionResult> ArtistList()
    {
        var artists = await _api.GetAsync<List<ArtistViewModel>>("api/artists");
        return artists is null ? RedirectToAction("SignIn", "Login") : View(artists);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var artist = await _api.GetAsync<ArtistViewModel>($"api/artists/{id}");
        return artist is null ? NotFound() : View(artist);
    }
}
