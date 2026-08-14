using JwtMusic.WebApi.Dtos;
using JwtMusic.WebApi.Entities;
using JwtMusic.WebApi.Services.LoginServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtMusic.WebApi.Services.LoginServices
{
    public class LoginService : ILoginService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;

        public LoginService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
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

            return await GenerateToken(user);
        }

        private async Task<string> GenerateToken(AppUser appUser)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
                new Claim(ClaimTypes.Email, appUser.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, appUser.UserName ?? string.Empty),
                new Claim("fullName", $"{appUser.Name} {appUser.Surname}".Trim()),
                new Claim(ClaimTypes.Surname, appUser.Surname),
                new Claim("package", appUser.PackageLevel.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpireMinutes"]!)),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
