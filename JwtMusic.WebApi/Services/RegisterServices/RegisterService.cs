using JwtMusic.WebApi.Dtos;
using JwtMusic.WebApi.Entities;
using Microsoft.AspNetCore.Identity;

namespace JwtMusic.WebApi.Services.RegisterServices
{
    public class RegisterService : IRegisterService
    {
        private readonly UserManager<AppUser> _userManager;

        public RegisterService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
        {
            AppUser appUser = new AppUser()
            {
                Email = dto.Email,
                Name = dto.Name,
                Surname = dto.Surname,
                UserName = dto.Username,
                ImageUrl = "/Bepop/assets/img/a0.jpg",
                PackageLevel = PackageLevel.Basic
            };

            return await _userManager.CreateAsync(appUser, dto.Password);
        }
    }
}
