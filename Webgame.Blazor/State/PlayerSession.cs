using System.Text;
using System.Text.Json;
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

    // Backward-compatible helper for old pages
    public async Task<Guid?> GetPlayerIdAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return null;

        try
        {
            var parts = token.Split('.');
            if (parts.Length < 2)
                return null;

            var payload = parts[1];
            var jsonBytes = DecodeBase64Url(payload);
            using var doc = JsonDocument.Parse(jsonBytes);

            // We write both NameIdentifier and sub into the token
            if (TryReadGuidClaim(doc, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", out var nameId))
                return nameId;

            if (TryReadGuidClaim(doc, "sub", out var sub))
                return sub;

            return null;
        }
        catch
        {
            return null;
        }
    }

    // Optional backward-compatible no-op wrapper so old code can compile if still calling it
    public Task SetPlayerIdAsync(Guid id)
        => Task.CompletedTask;

    private static bool TryReadGuidClaim(JsonDocument doc, string claimName, out Guid value)
    {
        value = Guid.Empty;

        if (!doc.RootElement.TryGetProperty(claimName, out var prop))
            return false;

        var text = prop.GetString();
        return Guid.TryParse(text, out value);
    }

    private static byte[] DecodeBase64Url(string input)
    {
        string s = input.Replace('-', '+').Replace('_', '/');

        switch (s.Length % 4)
        {
            case 2:
                s += "==";
                break;
            case 3:
                s += "=";
                break;
        }

        return Convert.FromBase64String(s);
    }
}