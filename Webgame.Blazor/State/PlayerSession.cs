using Microsoft.JSInterop;

namespace Webgame.Blazor.State;

public sealed class PlayerSession
{
    private const string Key = "webgame.playerId";
    private readonly IJSRuntime _js;

    public PlayerSession(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<Guid?> GetPlayerIdAsync()
    {
        var value = await _js.InvokeAsync<string?>("webgameStorage.get", Key);
        return Guid.TryParse(value, out var id) ? id : null;
    }

    public Task SetPlayerIdAsync(Guid id)
        => _js.InvokeVoidAsync("webgameStorage.set", Key, id.ToString()).AsTask();

    public Task ClearAsync()
        => _js.InvokeVoidAsync("webgameStorage.remove", Key).AsTask();
}