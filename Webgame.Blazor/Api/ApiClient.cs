using System.Net.Http.Json;
using Webgame.Contracts.Players;

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

    public async Task DeleteAsync()
    {
        var res = await _http.DeleteAsync("api/players/me");
        if (!res.IsSuccessStatusCode)
            await ReadOrThrowAsync<object>(res);
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
