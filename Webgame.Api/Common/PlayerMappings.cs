using Webgame.Api.Controllers;
using Webgame.Contracts.Players;
using Webgame.Domain.Players;

namespace Webgame.Api.Common;

public static class PlayerMappings
{
    public static PlayerResponse ToResponse(Player p)
        => new(
            p.Id.Value,
            p.Name,
            p.Stats.Level,
            p.Stats.Coins,
            p.Stats.ClickPower,
            p.Stats.ClickPowerLevel,
            p.Stats.AutoClickerLevel,
            p.Stats.TotalClicks,
            p.Stats.TotalCoinsEarned,
            p.Stats.TotalCoinsSpent
        );
}
