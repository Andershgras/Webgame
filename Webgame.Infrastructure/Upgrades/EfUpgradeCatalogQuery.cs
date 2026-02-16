using Microsoft.EntityFrameworkCore;
using Webgame.Application.Common;
using Webgame.Application.Upgrades;
using Webgame.Domain.Players;
using Webgame.Infrastructure.Persistence;

namespace Webgame.Infrastructure.Upgrades;

public sealed class EfUpgradeCatalogQuery : IUpgradeCatalogQuery
{
    private readonly WebgameDbContext _db;

    public EfUpgradeCatalogQuery(WebgameDbContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyList<UpgradeCatalogEntry>>> GetForPlayerAsync(Guid playerId, CancellationToken ct)
    {
        var pid = new PlayerId(playerId);

        var player = await _db.Players
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == pid, ct);

        if (player is null)
            return Result<IReadOnlyList<UpgradeCatalogEntry>>.Fail(Errors.PlayerNotFound);

        var cost = player.GetClickPowerUpgradeCost();

        IReadOnlyList<UpgradeCatalogEntry> list = new List<UpgradeCatalogEntry>
        {
            new(
                Key: "click_power",
                Name: "Click Power",
                CurrentLevel: player.Stats.ClickPowerLevel,
                NextCost: cost,
                EffectDescription: "+1 Click Power",
                CanAfford: player.Stats.Coins >= cost
            )
        };

        return Result<IReadOnlyList<UpgradeCatalogEntry>>.Ok(list);
    }
}
