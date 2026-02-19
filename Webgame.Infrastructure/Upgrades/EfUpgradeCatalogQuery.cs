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
        var cpcCost = player.GetCoinsPerClickUpgradeCost();
        var autoCost = player.GetAutoClickerUpgradeCost();

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
                Key: "coins_per_click",
                Name: "Coins Per Click",
                CurrentLevel: player.Stats.CoinsPerClickLevel,
                NextCost: cpcCost,
                EffectDescription: $"+1 bonus coin per click (now +{player.Stats.BonusCoinsPerClick})",
                CanAfford: player.Stats.Coins >= cpcCost
            ),
            new(
                Key: "auto_clicker",
                Name: "Auto Clicker",
                CurrentLevel: player.Stats.AutoClickerLevel,
                NextCost: autoCost,
                EffectDescription: $"+1 coin per tick (now {player.Stats.AutoCoinsPerTick}/tick)",
                CanAfford: player.Stats.Coins >= autoCost
            )
        };

        return Result<IReadOnlyList<UpgradeCatalogEntry>>.Ok(list);
    }
}
