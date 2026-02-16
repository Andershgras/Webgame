using Microsoft.AspNetCore.Mvc;
using Webgame.Api.Common;
using Webgame.Application.Upgrades;

namespace Webgame.Api.Controllers;

[ApiController]
[Route("api/players/{playerId:guid}/upgrades")]
public sealed class UpgradesController : ControllerBase
{
    private readonly IUpgradeCatalogQuery _query;

    public UpgradesController(IUpgradeCatalogQuery query)
    {
        _query = query;
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
}

