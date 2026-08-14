using JwtMusic.WebApi.Dtos;

using Microsoft.AspNetCore.Identity;

namespace JwtMusic.WebApi.Services.RegisterServices
{
    public interface IRegisterService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto);
    }
}
