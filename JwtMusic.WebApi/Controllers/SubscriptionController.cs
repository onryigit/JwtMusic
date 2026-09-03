using System.Security.Claims;
using JwtMusic.WebApi.Dtos;
using JwtMusic.WebApi.Entities;
using JwtMusic.WebApi.Services.TokenServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusic.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class SubscriptionController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenService _tokenService;

    public SubscriptionController(UserManager<AppUser> userManager, IJwtTokenService tokenService) =>
        (_userManager, _tokenService) = (userManager, tokenService);

    [HttpPost("upgrade")]
    public async Task<IActionResult> Upgrade(UpgradeSubscriptionDto dto)
    {
        if (!Enum.IsDefined(dto.NewTier))
            return BadRequest(new { message = "Geçersiz paket seçimi." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = userId is null ? null : await _userManager.FindByIdAsync(userId);
        if (user is null) return Unauthorized();

        if (dto.NewTier <= user.PlanTier)
            return BadRequest(new { message = "Yalnızca mevcut paketinizden daha üst bir pakete geçebilirsiniz." });

        user.PlanTier = dto.NewTier;
        var update = await _userManager.UpdateAsync(user);
        if (!update.Succeeded)
            return BadRequest(new { message = "Paket güncellenemedi.", errors = update.Errors.Select(x => x.Description) });

        var token = _tokenService.CreateToken(user);
        return Ok(new
        {
            token,
            message = "Paket başarıyla güncellendi.",
            planTier = (int)user.PlanTier,
            planTierName = user.PlanTier.ToString()
        });
    }
}
