using System.Net.Http.Json;
using JwtMusic.WebUI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusic.WebUI.Controllers;

public class RegisterController : Controller
{
    private readonly IHttpClientFactory _factory;
    private readonly IConfiguration _configuration;
    public RegisterController(IHttpClientFactory factory, IConfiguration configuration) =>
        (_factory, _configuration) = (factory, configuration);

    public IActionResult SignUp() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SignUp(RegisterDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        HttpResponseMessage response;
        try
        {
            response = await _factory.CreateClient().PostAsJsonAsync($"{_configuration["ApiBaseUrl"]}api/Register", dto);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty, "API'ye ulaşılamıyor. Önce WebApi projesini çalıştırın.");
            return View(dto);
        }

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Hesabınız oluşturuldu. Şimdi giriş yapabilirsiniz.";
            return RedirectToAction("SignIn", "Login");
        }

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError(string.Empty, error.Contains("DuplicateUserName") ? "Bu kullanıcı adı zaten kullanılıyor." :
            error.Contains("DuplicateEmail") ? "Bu e-posta zaten kullanılıyor." :
            "Kayıt yapılamadı. Bilgileri ve parola kurallarını kontrol edin.");
        return View(dto);
    }
}
