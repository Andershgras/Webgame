using Microsoft.JSInterop;

namespace Webgame.Blazor.State;

public sealed class PlayerSession
{
    private const string Key = "webgame.authToken";
    private readonly IJSRuntime _js;

    public PlayerSession(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _js.InvokeAsync<string?>("webgameStorage.get", Key);
    }

    public Task SetTokenAsync(string token)
        => _js.InvokeVoidAsync("webgameStorage.set", Key, token).AsTask();

    public Task ClearAsync()
        => _js.InvokeVoidAsync("webgameStorage.remove", Key).AsTask();
}