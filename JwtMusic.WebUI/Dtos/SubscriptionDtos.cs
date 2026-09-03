namespace JwtMusic.WebUI.Dtos;

public record UpgradeSubscriptionDto(int NewTier);

public sealed class UpgradeSubscriptionResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int PlanTier { get; set; }
    public string PlanTierName { get; set; } = string.Empty;
}
