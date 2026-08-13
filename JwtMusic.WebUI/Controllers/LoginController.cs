using System.Net.Http.Json;
using JwtMusic.WebUI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusic.WebUI.Controllers;

public class LoginController : Controller
{
    private readonly IHttpClientFactory _factory;
    private readonly IConfiguration _configuration;
    public LoginController(IHttpClientFactory factory, IConfiguration configuration) => (_factory, _configuration) = (factory, configuration);

    public IActionResult SignIn() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SignIn(LoginDto dto)
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync($"{_configuration["ApiBaseUrl"]}api/Login", dto);
        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Kullanıcı adı veya parola hatalı.");
            return View(dto);
        }
        var token = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        HttpContext.Session.SetString("JwtToken", token!.Token);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(SignIn));
    }
}
