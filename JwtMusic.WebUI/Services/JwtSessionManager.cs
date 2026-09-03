using System.Text.Json;

namespace JwtMusic.WebUI.Services;

public static class JwtSessionManager
{
    public static void Store(HttpContext context, string token, string? username = null)
    {
        context.Session.SetString("JwtToken", token);
        if (!string.IsNullOrWhiteSpace(username)) context.Session.SetString("Username", username);

        try
        {
            var payload = token.Split('.')[1].Replace('-', '+').Replace('_', '/');
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            using var json = JsonDocument.Parse(Convert.FromBase64String(payload));
            var root = json.RootElement;

            if (root.TryGetProperty("PlanTier", out var tier))
                context.Session.SetString("PlanTier", tier.GetString() ?? "1");
            if (root.TryGetProperty("PlanTierName", out var tierName))
            {
                var name = tierName.GetString() ?? "Basic";
                context.Session.SetString("PlanTierName", name);
                context.Session.SetString("Package", name);
            }
            if (root.TryGetProperty("fullName", out var fullName))
                context.Session.SetString("FullName", fullName.GetString() ?? string.Empty);
        }
        catch (Exception)
        {
            context.Session.SetString("PlanTier", "1");
            context.Session.SetString("PlanTierName", "Basic");
            context.Session.SetString("Package", "Basic");
        }
    }

    public static int GetTier(HttpContext context) =>
        int.TryParse(context.Session.GetString("PlanTier"), out var tier) && tier is >= 1 and <= 4 ? tier : 1;
}
