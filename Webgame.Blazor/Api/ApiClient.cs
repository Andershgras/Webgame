using System.Net.Http.Json;
using Webgame.Contracts.Players;
using Webgame.Contracts.Upgrades;

namespace Webgame.Blazor.Api;

public sealed class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<PlayerResponse> CreatePlayerAsync(string name)
    {
        var res = await _http.PostAsJsonAsync("api/players", new { name });
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<PlayerResponse>())!;
    }

    public async Task<PlayerResponse> GetPlayerAsync(Guid id)
        => await _http.GetFromJsonAsync<PlayerResponse>($"api/players/{id}")!;

    public async Task<PlayerResponse> ClickAsync(Guid id)
        => await _http.PostAsync($"api/players/{id}/click", null)
                      .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<PlayerResponse>()).Unwrap();

    public async Task<PlayerResponse> TickAsync(Guid id)
        => await _http.PostAsync($"api/players/{id}/tick", null)
                      .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<PlayerResponse>()).Unwrap();

    public async Task<IReadOnlyList<UpgradeCatalogEntry>> GetUpgradesAsync(Guid id)
        => await _http.GetFromJsonAsync<IReadOnlyList<UpgradeCatalogEntry>>(
            $"api/players/{id}/upgrades")!;

    public async Task<UpgradePurchaseResponse> BuyUpgradeAsync(Guid id, string key)
    {
        var res = await _http.PostAsync($"api/players/{id}/upgrades/{key}/buy", null);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<UpgradePurchaseResponse>())!;
    }
}