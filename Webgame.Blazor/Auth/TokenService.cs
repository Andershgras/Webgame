using Microsoft.JSInterop;

namespace Webgame.Blazor.Auth;

public sealed class TokenService
{
    private const string Key = "auth_token";
    private readonly IJSRuntime _js;

    public TokenService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task SetTokenAsync(string token)
    {
        await _js.InvokeVoidAsync("webgameStorage.set", Key, token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _js.InvokeAsync<string?>("webgameStorage.get", Key);
    }

    public async Task RemoveTokenAsync()
    {
        await _js.InvokeVoidAsync("webgameStorage.remove", Key);
    }
}