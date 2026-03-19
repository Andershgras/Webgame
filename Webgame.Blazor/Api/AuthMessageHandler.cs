using System.Net;
using System.Net.Http.Headers;
using Webgame.Blazor.State;

namespace Webgame.Blazor.Api;

public sealed class AuthMessageHandler : DelegatingHandler
{
    private readonly PlayerSession _session;

    public AuthMessageHandler(PlayerSession session)
    {
        _session = session;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _session.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await _session.ClearAsync();
        }

        return response;
    }
}