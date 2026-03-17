using Microsoft.EntityFrameworkCore;
using Webgame.Application.Common;
using Webgame.Application.Upgrades;
using Webgame.Domain.Players;
using Webgame.Infrastructure.Persistence;
using Webgame.Contracts.Upgrades;

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
        var autoCost = player.GetAutoClickerUpgradeCost();
        var offlineCost = player.GetOfflineCapUpgradeCost();
        IReadOnlyList<UpgradeCatalogEntry> list = new List<UpgradeCatalogEntry>
        {
            new(
                Key: "click_power",
                Name: "Click Power",
                CurrentLevel: player.Stats.ClickPowerLevel,
                NextCost: cost,
                EffectDescription: "+1 Click Power",
                CanAfford: player.Stats.Coins >= cost
            ),
            new(
                Key: "auto_clicker",
                Name: "Auto Clicker",
                CurrentLevel: player.Stats.AutoClickerLevel,
                NextCost: autoCost,
                EffectDescription: $"+1 coin per tick (now {player.Stats.AutoCoinsPerTick}/tick)",
                CanAfford: player.Stats.Coins >= autoCost
            ),
            new(
                Key: "offline_cap",
                Name: "Offline Cap",
                CurrentLevel: player.Stats.OfflineCapLevel,
                NextCost: offlineCost,
                EffectDescription: $"+30 min offline cap (now {player.Stats.OfflineCapSeconds / 60} min)",
                CanAfford: player.Stats.Coins >= offlineCost
            ),
        };

        return Result<IReadOnlyList<UpgradeCatalogEntry>>.Ok(list);
    }
}
