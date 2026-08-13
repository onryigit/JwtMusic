using System.Net.Http.Json;
using JwtMusic.WebUI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusic.WebUI.Controllers;

public class RegisterController : Controller
{
    private readonly IHttpClientFactory _factory;
    private readonly IConfiguration _configuration;
    public RegisterController(IHttpClientFactory factory, IConfiguration configuration) => (_factory, _configuration) = (factory, configuration);

    public IActionResult SignUp() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SignUp(RegisterDto dto)
    {
        var response = await _factory.CreateClient().PostAsJsonAsync($"{_configuration["ApiBaseUrl"]}api/Register", dto);
        if (response.IsSuccessStatusCode) return RedirectToAction("SignIn", "Login");
        ModelState.AddModelError(string.Empty, "Kayıt yapılamadı. Bilgileri ve parola kurallarını kontrol edin.");
        return View(dto);
    }
}
