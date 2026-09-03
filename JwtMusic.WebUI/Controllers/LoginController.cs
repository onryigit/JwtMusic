using System.Net.Http.Json;
using JwtMusic.WebUI.Dtos;
using JwtMusic.WebUI.Services;
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

        JwtSessionManager.Store(HttpContext, token.Token, dto.Username);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(SignIn));
    }

}
