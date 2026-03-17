using Microsoft.AspNetCore.Mvc;
using Webgame.Api.Common;
using Webgame.Application.Players;
using Webgame.Application.Upgrades;
using Webgame.Contracts.Upgrades;
using Webgame.Contracts.Players;

namespace Webgame.Api.Controllers;

[ApiController]
[Route("api/players/{playerId:guid}/upgrades")]
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
    public async Task<ActionResult<IReadOnlyList<UpgradeCatalogEntry>>> Get(Guid playerId, CancellationToken ct)
    {
        var result = await _query.GetForPlayerAsync(playerId, ct);

        return ResultToHttp.ToActionResult<
            IReadOnlyList<UpgradeCatalogEntry>,
            IReadOnlyList<UpgradeCatalogEntry>>(
            this,
            result,
            x => x,
            dto => Ok(dto));
    }
    [HttpPost("{key}/buy")]
    public async Task<ActionResult<UpgradePurchaseResponse>> Buy(Guid playerId, string key, CancellationToken ct)
    {
        var result = await _players.BuyUpgradeAsync(new Webgame.Domain.Players.PlayerId(playerId), key, ct);

        return ResultToHttp.ToActionResult<Webgame.Application.Upgrades.UpgradePurchaseResult, UpgradePurchaseResponse>(
            this,
            result,
            r => new UpgradePurchaseResponse(
                r.Key,
                r.Cost,
                r.NewLevel,
                PlayerMappings.ToResponse(r.Player)
            ),
            dto => Ok(dto)
        );
    }
}

