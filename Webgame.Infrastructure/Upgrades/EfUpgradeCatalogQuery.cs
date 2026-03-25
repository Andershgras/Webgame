using Microsoft.EntityFrameworkCore;
using Webgame.Application.Common;
using Webgame.Application.Upgrades;
using Webgame.Contracts.Upgrades;
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

        var fasterCoresMaxed = player.Stats.FasterCoresLevel >= Stats.MaxFasterCoresLevel;
        var betterCoresMaxed = player.Stats.BetterCoresLevel >= Stats.MaxBetterCoresLevel;
        var betterCores2Maxed = player.Stats.BetterCores2Level >= Stats.MaxBetterCores2Level;
        var offlineProductionMaxed = player.Stats.OfflineProductionLevel >= Stats.MaxOfflineProductionLevel;
        var moreStellarEnergyMaxed = player.Stats.MoreStellarEnergyLevel >= Stats.MaxMoreStellarEnergyLevel;

        var fasterLevelUpMaxed = player.Stats.FasterLevelUpLevel >= Stats.MaxFasterLevelUpLevel;
        var stellarMoreStellarEnergyMaxed = player.Stats.StellarMoreStellarEnergyLevel >= Stats.MaxStellarMoreStellarEnergyLevel;

        var offlineCapCost = player.GetOfflineCapUpgradeCost();
        var fasterCoresCost = player.GetFasterCoresUpgradeCost();
        var betterCoresCost = player.GetBetterCoresUpgradeCost();
        var betterCores2Cost = player.GetBetterCores2UpgradeCost();
        var offlineProductionCost = player.GetOfflineProductionUpgradeCost();
        var moreStellarEnergyCost = player.GetMoreStellarEnergyUpgradeCost();

        var fasterLevelUpCost = player.GetFasterLevelUpUpgradeCost();
        var stellarMoreStellarEnergyCost = player.GetStellarMoreStellarEnergyUpgradeCost();

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
                CanAfford: !fasterCoresMaxed && player.Stats.Energy >= fasterCoresCost,
                CurrencyType: "energy"
            ),

            new(
                Key: "better_cores",
                Name: "Better Cores",
                CurrentLevel: player.Stats.BetterCoresLevel,
                NextCost: betterCoresMaxed ? 0 : betterCoresCost,
                EffectDescription: betterCoresMaxed
                    ? $"MAX - base spawn tier is {player.GetBaseSpawnTier()}"
                    : $"+1 base spawn tier (now tier {player.GetBaseSpawnTier()} / max {Stats.MaxBetterCoresLevel})",
                CanAfford: !betterCoresMaxed && player.Stats.Energy >= betterCoresCost,
                CurrencyType: "energy"
            ),

            new(
                Key: "better_cores_2",
                Name: "Better Cores 2",
                CurrentLevel: player.Stats.BetterCores2Level,
                NextCost: betterCores2Maxed ? 0 : betterCores2Cost,
                EffectDescription: betterCores2Maxed
                    ? $"MAX - {player.Stats.BetterCores2Level}% chance for +1 extra tier"
                    : $"+1% chance for +1 extra tier (now {player.Stats.BetterCores2Level}% / max {Stats.MaxBetterCores2Level}%)",
                CanAfford: !betterCores2Maxed && player.Stats.Energy >= betterCores2Cost,
                CurrencyType: "energy"
            ),

            new(
                Key: "offline_production",
                Name: "Offline Production",
                CurrentLevel: player.Stats.OfflineProductionLevel,
                NextCost: offlineProductionMaxed ? 0 : offlineProductionCost,
                EffectDescription: offlineProductionMaxed
                    ? $"MAX - offline production is +{player.Stats.OfflineProductionLevel}%"
                    : $"+1% offline production (now +{player.Stats.OfflineProductionLevel}% / max {Stats.MaxOfflineProductionLevel}%)",
                CanAfford: !offlineProductionMaxed && player.Stats.Energy >= offlineProductionCost,
                CurrencyType: "energy"
            ),

            new(
                Key: "more_stellar_energy",
                Name: "More Stellar Energy",
                CurrentLevel: player.Stats.MoreStellarEnergyLevel,
                NextCost: moreStellarEnergyMaxed ? 0 : moreStellarEnergyCost,
                EffectDescription: moreStellarEnergyMaxed
                    ? $"MAX - {player.GetStellarEnergyMergeChance() * 100:0.0}% merge chance"
                    : $"+0.1% Stellar Energy chance on merge (now {player.GetStellarEnergyMergeChance() * 100:0.0}% / max {Stats.MaxMoreStellarEnergyLevel})",
                CanAfford: !moreStellarEnergyMaxed && player.Stats.Energy >= moreStellarEnergyCost,
                CurrencyType: "energy"
            ),

            new(
                Key: "offline_cap",
                Name: "Offline Cap",
                CurrentLevel: player.Stats.OfflineCapLevel,
                NextCost: offlineCapCost,
                EffectDescription: $"+30 min offline cap (now {player.Stats.OfflineCapSeconds / 60} min)",
                CanAfford: player.Stats.Energy >= offlineCapCost,
                CurrencyType: "energy"
            ),

            new(
                Key: "faster_level_up",
                Name: "Faster Level Up",
                CurrentLevel: player.Stats.FasterLevelUpLevel,
                NextCost: fasterLevelUpMaxed ? 0 : fasterLevelUpCost,
                EffectDescription: fasterLevelUpMaxed
                    ? $"MAX - {player.GetMergeXpMultiplier():0.0}x merge XP"
                    : $"+10% merge XP (now {player.GetMergeXpMultiplier():0.0}x / max {Stats.MaxFasterLevelUpLevel})",
                CanAfford: !fasterLevelUpMaxed && player.Stats.StellarEnergy >= fasterLevelUpCost,
                CurrencyType: "stellar"
            ),

            new(
                Key: "stellar_more_stellar_energy",
                Name: "More Stellar Energy",
                CurrentLevel: player.Stats.StellarMoreStellarEnergyLevel,
                NextCost: stellarMoreStellarEnergyMaxed ? 0 : stellarMoreStellarEnergyCost,
                EffectDescription: stellarMoreStellarEnergyMaxed
                    ? $"MAX - {player.GetStellarEnergySpawnChance() * 100:0.0}% spawn chance"
                    : $"+2% Stellar Energy chance on spawn (now {player.GetStellarEnergySpawnChance() * 100:0.0}% / max {Stats.MaxStellarMoreStellarEnergyLevel})",
                CanAfford: !stellarMoreStellarEnergyMaxed && player.Stats.StellarEnergy >= stellarMoreStellarEnergyCost,
                CurrencyType: "stellar"
            )
        };

        return Result<IReadOnlyList<UpgradeCatalogEntry>>.Ok(list);
    }
}