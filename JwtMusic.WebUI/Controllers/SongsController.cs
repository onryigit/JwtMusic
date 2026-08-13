using JwtMusic.WebUI.Models;
using JwtMusic.WebUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusic.WebUI.Controllers;

public class SongsController : Controller
{
    private readonly MusicApiClient _api;
    public SongsController(MusicApiClient api) => _api = api;

    public async Task<IActionResult> Detail(int id)
    {
        var model = await _api.GetAsync<SongDetailViewModel>($"api/songs/{id}");
        return model is null ? NotFound() : View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Play(int id)
    {
        using var response = await _api.StreamAsync(id);
        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            return StatusCode(403, await response.Content.ReadAsStringAsync());
        if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);
        return File(await response.Content.ReadAsByteArrayAsync(), "audio/mpeg", enableRangeProcessing: true);
    }
}
