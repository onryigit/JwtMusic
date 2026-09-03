using JwtMusic.WebApi.Dtos;
using JwtMusic.WebApi.Entities;
using JwtMusic.WebApi.Services.LoginServices;
using JwtMusic.WebApi.Services.TokenServices;
using Microsoft.AspNetCore.Identity;

namespace JwtMusic.WebApi.Services.LoginServices
{
    public class LoginService : ILoginService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtTokenService _tokenService;

        public LoginService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtTokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<string?> LoginAsync(LoginDto loginDto)
        {
            var value = loginDto.Username.Trim();
            var user = value.Contains('@')
                ? await _userManager.FindByEmailAsync(value)
                : await _userManager.FindByNameAsync(value);

            if (user == null)
            {
                return null;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return null;
            }

            return _tokenService.CreateToken(user);
        }
    }
}
