using System.Net.Http.Json;
using System.Text.Json;
using JwtMusic.WebUI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusic.WebUI.Controllers;

public class LoginController : Controller
{
    private readonly IHttpClientFactory _factory;
    private readonly IConfiguration _configuration;
    public LoginController(IHttpClientFactory factory, IConfiguration configuration) =>
        (_factory, _configuration) = (factory, configuration);

    public IActionResult SignIn() => HttpContext.Session.GetString("JwtToken") is not null
        ? RedirectToAction("Index", "Home") : View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SignIn(LoginDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        HttpResponseMessage response;
        try
        {
            response = await _factory.CreateClient().PostAsJsonAsync($"{_configuration["ApiBaseUrl"]}api/Login", dto);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty, "API'ye ulaşılamıyor. Önce WebApi projesini çalıştırın.");
            return View(dto);
        }

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Kullanıcı adı/e-posta veya parola hatalı.");
            return View(dto);
        }

        var token = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        if (string.IsNullOrWhiteSpace(token?.Token))
        {
            ModelState.AddModelError(string.Empty, "API geçerli bir JWT üretmedi.");
            return View(dto);
        }

        HttpContext.Session.SetString("JwtToken", token.Token);
        HttpContext.Session.SetString("Username", dto.Username);
        StorePackageClaim(token.Token);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(SignIn));
    }

    private void StorePackageClaim(string token)
    {
        try
        {
            var payload = token.Split('.')[1].Replace('-', '+').Replace('_', '/');
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            using var json = JsonDocument.Parse(Convert.FromBase64String(payload));
            if (json.RootElement.TryGetProperty("package", out var package))
                HttpContext.Session.SetString("Package", package.GetString() ?? "Basic");
            if (json.RootElement.TryGetProperty("fullName", out var name))
                HttpContext.Session.SetString("FullName", name.GetString() ?? string.Empty);
        }
        catch (Exception) { HttpContext.Session.SetString("Package", "Basic"); }
    }
}
