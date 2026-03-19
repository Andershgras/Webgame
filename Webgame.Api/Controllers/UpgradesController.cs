using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Webgame.Api.Common;
using Webgame.Application.Players;
using Webgame.Application.Upgrades;
using Webgame.Contracts.Players;
using Webgame.Contracts.Upgrades;
using Webgame.Domain.Players;

namespace Webgame.Api.Controllers;

[ApiController]
[Route("api/players/me/upgrades")]
[Authorize]
public sealed class UpgradesController : ControllerBase
{
    private readonly IUpgradeCatalogQuery _query;
    private readonly PlayerService _players;

    public UpgradesController(IUpgradeCatalogQuery query, PlayerService players)
    {
        _query = query;
        _players = players;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UpgradeCatalogEntry>>> Get(CancellationToken ct)
    {
        var playerId = GetPlayerIdFromToken();
        if (playerId is null)
            return Unauthorized();

        var result = await _query.GetForPlayerAsync(playerId.Value.Value, ct);

        return ResultToHttp.ToActionResult<
            IReadOnlyList<UpgradeCatalogEntry>,
            IReadOnlyList<UpgradeCatalogEntry>>(
            this,
            result,
            x => x,
            dto => Ok(dto));
    }

    [HttpPost("{key}/buy")]
    public async Task<ActionResult<UpgradePurchaseResponse>> Buy(string key, CancellationToken ct)
    {
        var playerId = GetPlayerIdFromToken();
        if (playerId is null)
            return Unauthorized();

        var result = await _players.BuyUpgradeAsync(playerId.Value, key, ct);

        return ResultToHttp.ToActionResult<Webgame.Application.Upgrades.UpgradePurchaseResult, UpgradePurchaseResponse>(
            this,
            result,
            r => new UpgradePurchaseResponse(
                r.Key,
                r.Cost,
                r.NewLevel,
                PlayerMappings.ToResponse(r.Player)
            ),
            dto => Ok(dto));
    }

    private PlayerId? GetPlayerIdFromToken()
    {
        var claimValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub");

        if (!Guid.TryParse(claimValue, out var id))
            return null;

        return new PlayerId(id);
    }
}