using System.Net.Http.Json;
using JwtMusic.WebUI.Dtos;
using JwtMusic.WebUI.Models;
using JwtMusic.WebUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusic.WebUI.Controllers;

public sealed class SubscriptionController : Controller
{
    private readonly MusicApiClient _api;

    public SubscriptionController(MusicApiClient api) => _api = api;

    [HttpGet]
    public IActionResult Pricing()
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("JwtToken")))
            return RedirectToAction("SignIn", "Login");

        var currentTier = JwtSessionManager.GetTier(HttpContext);
        var currentName = HttpContext.Session.GetString("PlanTierName") ?? "Basic";
        return View(new PricingViewModel(currentTier, currentName));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upgrade([FromBody] UpgradeSubscriptionDto dto)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("JwtToken"))) return Unauthorized();

        try
        {
            using var response = await _api.PostAsync("api/subscription/upgrade", dto);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return new ContentResult
                {
                    StatusCode = (int)response.StatusCode,
                    ContentType = "application/json",
                    Content = error
                };
            }

            var result = await response.Content.ReadFromJsonAsync<UpgradeSubscriptionResponseDto>();
            if (result is null || string.IsNullOrWhiteSpace(result.Token))
                return StatusCode(502, new { message = "API geçerli bir token döndürmedi." });

            JwtSessionManager.Store(HttpContext, result.Token);
            return Json(result);
        }
        catch (HttpRequestException)
        {
            return StatusCode(503, new { message = "Paket servisine şu anda ulaşılamıyor." });
        }
    }
}
