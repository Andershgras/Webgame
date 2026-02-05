using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Webgame.Application.Players;
using Webgame.Domain.Players;

namespace Webgame.Infrastructure.Players;

public sealed class InMemoryPlayerRepository : IPlayerRepository
{
    private static readonly ConcurrentDictionary<Guid, Player> _players = new();

    public Task AddAsync(Player player, CancellationToken ct)
    {
        _players[player.Id.Value] = player;
        return Task.CompletedTask;
    }

    public Task<Player?> GetByIdAsync(PlayerId id, CancellationToken ct)
    {
        _players.TryGetValue(id.Value, out var player);
        return Task.FromResult(player);
    }
}

