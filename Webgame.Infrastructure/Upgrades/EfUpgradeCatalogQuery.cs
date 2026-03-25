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

        var fasterCoresMaxed = player.Stats.FasterCoresLevel >= Stats.MaxFasterCoresLevel;
        var betterCoresMaxed = player.Stats.BetterCoresLevel >= Stats.MaxBetterCoresLevel;
        var betterCores2Maxed = player.Stats.BetterCores2Level >= Stats.MaxBetterCores2Level;
        var offlineProductionMaxed = player.Stats.OfflineProductionLevel >= Stats.MaxOfflineProductionLevel;

        var offlineCapCost = player.GetOfflineCapUpgradeCost();
        var fasterCoresCost = player.GetFasterCoresUpgradeCost();
        var betterCoresCost = player.GetBetterCoresUpgradeCost();
        var betterCores2Cost = player.GetBetterCores2UpgradeCost();
        var offlineProductionCost = player.GetOfflineProductionUpgradeCost();

        IReadOnlyList<UpgradeCatalogEntry> list = new List<UpgradeCatalogEntry>
        {
            new(
                Key: "faster_cores",
                Name: "Faster Cores",
                CurrentLevel: player.Stats.FasterCoresLevel,
                NextCost: fasterCoresMaxed ? 0 : fasterCoresCost,
                EffectDescription: fasterCoresMaxed
                    ? $"MAX - spawn interval is {player.GetSpawnIntervalSeconds():0.0}s"
                    : $"-0.1s spawn interval (now {player.GetSpawnIntervalSeconds():0.0}s / max {Stats.MaxFasterCoresLevel})",
                CanAfford: !fasterCoresMaxed && player.Stats.Energy >= fasterCoresCost
            ),

            new(
                Key: "better_cores",
                Name: "Better Cores",
                CurrentLevel: player.Stats.BetterCoresLevel,
                NextCost: betterCoresMaxed ? 0 : betterCoresCost,
                EffectDescription: betterCoresMaxed
                    ? $"MAX - base spawn tier is {player.GetBaseSpawnTier()}"
                    : $"+1 base spawn tier (now tier {player.GetBaseSpawnTier()} / max {Stats.MaxBetterCoresLevel})",
                CanAfford: !betterCoresMaxed && player.Stats.Energy >= betterCoresCost
            ),

            new(
                Key: "better_cores_2",
                Name: "Better Cores 2",
                CurrentLevel: player.Stats.BetterCores2Level,
                NextCost: betterCores2Maxed ? 0 : betterCores2Cost,
                EffectDescription: betterCores2Maxed
                    ? $"MAX - {player.Stats.BetterCores2Level}% chance for +1 extra tier"
                    : $"+1% chance for +1 extra tier (now {player.Stats.BetterCores2Level}% / max {Stats.MaxBetterCores2Level}%)",
                CanAfford: !betterCores2Maxed && player.Stats.Energy >= betterCores2Cost
            ),

            new(
                Key: "offline_production",
                Name: "Offline Production",
                CurrentLevel: player.Stats.OfflineProductionLevel,
                NextCost: offlineProductionMaxed ? 0 : offlineProductionCost,
                EffectDescription: offlineProductionMaxed
                    ? $"MAX - offline production is +{player.Stats.OfflineProductionLevel}%"
                    : $"+1% offline production (now +{player.Stats.OfflineProductionLevel}% / max {Stats.MaxOfflineProductionLevel}%)",
                CanAfford: !offlineProductionMaxed && player.Stats.Energy >= offlineProductionCost
            ),

            new(
                Key: "offline_cap",
                Name: "Offline Cap",
                CurrentLevel: player.Stats.OfflineCapLevel,
                NextCost: offlineCapCost,
                EffectDescription: $"+30 min offline cap (now {player.Stats.OfflineCapSeconds / 60} min)",
                CanAfford: player.Stats.Energy >= offlineCapCost
            )
        };

        return Result<IReadOnlyList<UpgradeCatalogEntry>>.Ok(list);
    }
}