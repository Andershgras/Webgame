using System.Linq;
using Webgame.Contracts.Players;
using Webgame.Domain.Players;

namespace Webgame.Api.Common;

public static class PlayerMappings
{
    public static PlayerResponse ToResponse(Player p)
    {
        var offline = p.ConsumeOfflineProgress();

        var cores = p.Board.Cores
            .OrderBy(c => c.SlotIndex)
            .Select(c => new CoreDto(
                c.Id,
                c.Tier,
                c.SlotIndex,
                c.GetProductionPerSecond()))
            .ToList();

        return new PlayerResponse(
            p.Id.Value,
            p.Name,

            p.Stats.Level,
            p.Stats.Experience,
            p.Stats.ExperienceToNextLevel,

            p.Stats.Energy,
            p.Stats.StellarEnergy,
            p.Stats.StormEnergy,

            p.Board.SlotCount,
            cores,

            // Temporary legacy fields
            p.Stats.ClickPower,
            p.Stats.ClickPowerLevel,
            p.Stats.AutoClickerLevel,
            p.Stats.ClicksPerSecond,

            p.Stats.OfflineCapSeconds,
            offline.EnergyEarned,
            offline.SecondsApplied,

            p.Stats.TotalClicks,
            p.Stats.TotalEnergyEarned,
            p.Stats.TotalEnergySpent,
            p.Stats.TotalMerges
        );
    }
}