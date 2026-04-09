using Webgame.Contracts.Players;
using Webgame.Domain.Players;

namespace Webgame.Api.Common;

public static class PlayerMappings
{
    public static PlayerResponse ToResponse(Player p)
    {
        return new PlayerResponse(p.Id.Value, p.Name, p.Stats.Currency);
    }
}
