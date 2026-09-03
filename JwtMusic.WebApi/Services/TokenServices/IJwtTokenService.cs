using JwtMusic.WebApi.Entities;

namespace JwtMusic.WebApi.Services.TokenServices;

public interface IJwtTokenService
{
    string CreateToken(AppUser user);
}
