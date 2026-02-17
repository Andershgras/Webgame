using Webgame.Api.Controllers;
using Webgame.Domain.Players;

namespace Webgame.Api.Common;

public static class PlayerMappings
{
    public static PlayersController.PlayerResponse ToResponse(Player p)
        => new(
            p.Id.Value,
            p.Name,
            p.Stats.Level,
            p.Stats.Coins,
            p.Stats.ClickPower,
            p.Stats.ClickPowerLevel
        );
}
