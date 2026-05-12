using JwtMusic.WebApi.Dtos;
using JwtMusic.WebApi.Services.RegisterServices;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService _registerService;

        public RegisterController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterDto registerDto)
        {
            var result = await _registerService.RegisterAsync(registerDto);
            if (!result)
                return BadRequest("Kayıt başarısız!");
            return Ok("Başarılı!");
        }
    }
}