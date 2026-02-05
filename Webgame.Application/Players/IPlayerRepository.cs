using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webgame.Domain.Players;

namespace Webgame.Application.Players;

public interface IPlayerRepository
{
    Task AddAsync(Player player, CancellationToken ct);
    Task<Player?> GetByIdAsync(PlayerId id, CancellationToken ct);
}
