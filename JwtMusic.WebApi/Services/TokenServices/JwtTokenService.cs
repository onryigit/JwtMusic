using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtMusic.WebApi.Entities;
using Microsoft.IdentityModel.Tokens;

namespace JwtMusic.WebApi.Services.TokenServices;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration) => _configuration = configuration;

    public string CreateToken(AppUser user)
    {
        var settings = _configuration.GetSection("JwtSettings");
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Surname, user.Surname),
            new("fullName", $"{user.Name} {user.Surname}".Trim()),
            new("PlanTier", ((int)user.PlanTier).ToString()),
            new("PlanTierName", user.PlanTier.ToString()),
            new("package", user.PlanTier.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings["Key"]!));
        var token = new JwtSecurityToken(
            issuer: settings["Issuer"],
            audience: settings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(settings["ExpireMinutes"]!)),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
