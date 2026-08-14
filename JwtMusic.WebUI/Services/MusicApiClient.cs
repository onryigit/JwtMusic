using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace JwtMusic.WebUI.Services;

public class MusicApiClient
{
    private readonly HttpClient _client;
    private readonly IHttpContextAccessor _context;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public MusicApiClient(HttpClient client, IHttpContextAccessor context) => (_client, _context) = (client, context);

    public async Task<T?> GetAsync<T>(string url)
    {
        using var request = CreateRequest(HttpMethod.Get, url);
        using var response = await _client.SendAsync(request);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) return default;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    public async Task<HttpResponseMessage> StreamAsync(int id) =>
        await _client.SendAsync(CreateRequest(HttpMethod.Get, $"api/songs/{id}/stream"), HttpCompletionOption.ResponseHeadersRead);

    public async Task<HttpResponseMessage> PostAsync<T>(string url, T value)
    {
        using var request = CreateRequest(HttpMethod.Post, url);
        request.Content = JsonContent.Create(value, options: JsonOptions);
        return await _client.SendAsync(request);
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        var token = _context.HttpContext?.Session.GetString("JwtToken");
        if (!string.IsNullOrWhiteSpace(token)) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }
}
