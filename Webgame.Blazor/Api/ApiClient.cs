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

    public async Task<LoginResponse> LoginAsync(string name, string password)
    {
        var res = await _http.PostAsJsonAsync("api/players/login", new { name, password });
        return await ReadOrThrowAsync<LoginResponse>(res);
    }

    public async Task<PlayerResponse> RegisterAsync(string name, string password)
    {
        var res = await _http.PostAsJsonAsync("api/players", new { name, password });
        return await ReadOrThrowAsync<PlayerResponse>(res);
    }

    public async Task<PlayerResponse> GetMeAsync()
    {
        var res = await _http.GetAsync("api/players/me");
        return await ReadOrThrowAsync<PlayerResponse>(res);
    }

    public async Task<PlayerResponse> ClickAsync()
    {
        var res = await _http.PostAsync("api/players/me/click", null);
        return await ReadOrThrowAsync<PlayerResponse>(res);
    }

    public async Task<PlayerResponse> TickAsync()
    {
        var res = await _http.PostAsync("api/players/me/tick", null);
        return await ReadOrThrowAsync<PlayerResponse>(res);
    }

    public async Task DeleteAsync()
    {
        var res = await _http.DeleteAsync("api/players/me");
        if (!res.IsSuccessStatusCode)
            await ReadOrThrowAsync<object>(res);
    }

    public async Task<IReadOnlyList<UpgradeCatalogEntry>> GetUpgradesAsync()
    {
        var res = await _http.GetAsync("api/players/me/upgrades");
        return await ReadOrThrowAsync<IReadOnlyList<UpgradeCatalogEntry>>(res);
    }

    public async Task<UpgradePurchaseResponse> BuyUpgradeAsync(string key)
    {
        var res = await _http.PostAsync($"api/players/me/upgrades/{key}/buy", null);
        return await ReadOrThrowAsync<UpgradePurchaseResponse>(res);
    }

    public async Task<IReadOnlyList<LeaderboardEntry>> GetLeaderboardAsync(int top = 10, LeaderboardType type = LeaderboardType.TotalClicks)
    {
        var res = await _http.GetAsync($"api/leaderboard?top={top}&type={Uri.EscapeDataString(type.ToString())}");
        return await ReadOrThrowAsync<IReadOnlyList<LeaderboardEntry>>(res);
    }

    public async Task<int> GetLeaderboardRankAsync(Guid playerId, LeaderboardType type = LeaderboardType.TotalClicks)
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
        }

        throw new ApiException(res.StatusCode, problem);
    }
}