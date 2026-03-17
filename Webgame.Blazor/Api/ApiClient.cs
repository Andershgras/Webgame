using System.Net;
using System.Net.Http.Json;
using Webgame.Contracts.Leaderboards;
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
        return await ReadOrThrowAsync<PlayerResponse>(res);
    }

    public async Task<PlayerResponse> GetPlayerAsync(Guid id)
    {
        var res = await _http.GetAsync($"api/players/{id}");
        return await ReadOrThrowAsync<PlayerResponse>(res);
    }

    public async Task<PlayerResponse> ClickAsync(Guid id)
    {
        var res = await _http.PostAsync($"api/players/{id}/click", null);
        return await ReadOrThrowAsync<PlayerResponse>(res);
    }

    public async Task<PlayerResponse> TickAsync(Guid id)
    {
        var res = await _http.PostAsync($"api/players/{id}/tick", null);
        return await ReadOrThrowAsync<PlayerResponse>(res);
    }

    public async Task<IReadOnlyList<UpgradeCatalogEntry>> GetUpgradesAsync(Guid id)
    {
        var res = await _http.GetAsync($"api/players/{id}/upgrades");
        return await ReadOrThrowAsync<IReadOnlyList<UpgradeCatalogEntry>>(res);
    }

    public async Task<UpgradePurchaseResponse> BuyUpgradeAsync(Guid id, string key)
    {
        var res = await _http.PostAsync($"api/players/{id}/upgrades/{key}/buy", null);
        return await ReadOrThrowAsync<UpgradePurchaseResponse>(res);
    }

    public async Task<IReadOnlyList<LeaderboardEntry>> GetLeaderboardAsync(int top = 10, LeaderboardType type = LeaderboardType.Coins)
    {
        var res = await _http.GetAsync($"api/leaderboard?top={top}&type={Uri.EscapeDataString(type.ToString())}");
        return await ReadOrThrowAsync<IReadOnlyList<LeaderboardEntry>>(res);
    }

    public async Task<int> GetLeaderboardRankAsync(Guid playerId, LeaderboardType type = LeaderboardType.Coins)
    {
        var res = await _http.GetAsync($"api/leaderboard/rank?playerId={playerId}&type={Uri.EscapeDataString(type.ToString())}");
        return await ReadOrThrowAsync<int>(res);
    }

    private async Task<T> ReadOrThrowAsync<T>(HttpResponseMessage res)
    {
        if (res.IsSuccessStatusCode)
        {
            var ok = await res.Content.ReadFromJsonAsync<T>();
            return ok!;
        }

        ApiProblemDetails? problem = null;

        try
        {
            problem = await res.Content.ReadFromJsonAsync<ApiProblemDetails>();
        }
        catch
        {
            // ignore parse issues
        }

        throw new ApiException(res.StatusCode, problem);
    }
}