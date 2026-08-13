
    using JwtMusic.WebApi.Dtos;
    using JwtMusic.WebApi.Services.LoginServices;
    using Microsoft.AspNetCore.Mvc;

    namespace JwtMusicNight.WebApi.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class LoginController : ControllerBase
        {
            private readonly ILoginService _loginService;

            public LoginController(ILoginService loginService)
            {
                _loginService = loginService;
            }

            [HttpPost]
            public async Task<IActionResult> UserLogin(LoginDto loginDto)
            {
                var token = await _loginService.LoginAsync(loginDto);
                return token is null ? Unauthorized(new { message = "Kullanıcı adı veya parola hatalı." }) : Ok(new { token });
            }
        }
    }
