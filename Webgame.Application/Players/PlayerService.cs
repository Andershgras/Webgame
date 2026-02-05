using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Webgame.Domain.Players;

namespace Webgame.Application.Players;

public sealed class PlayerService
{
    private readonly IPlayerRepository _repo;

    public PlayerService(IPlayerRepository repo)
    {
        _repo = repo;
    }

    public async Task<Player> CreatePlayerAsync(string name, CancellationToken ct)
    {
        var player = Player.CreateNew(name);
        await _repo.AddAsync(player, ct);
        return player;
    }

    public Task<Player?> GetPlayerAsync(PlayerId id, CancellationToken ct)
        => _repo.GetByIdAsync(id, ct);

    public async Task<Player?> ClickAsync(PlayerId id, CancellationToken ct)
    {
        var player = await _repo.GetByIdAsync(id, ct);
        if (player is null) return null;

        player.Click();

        await _repo.AddAsync(player, ct);

        return player;
    }
}

