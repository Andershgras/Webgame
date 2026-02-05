using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Webgame.Domain.Players;
using Webgame.Application.Persistence;

namespace Webgame.Application.Players;

public sealed class PlayerService
{
    private readonly IPlayerRepository _repo;
    private readonly IUnitOfWork _uow;

    public PlayerService(IPlayerRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Player> CreatePlayerAsync(string name, CancellationToken ct)
    {
        var player = Player.CreateNew(name);

        await _repo.AddAsync(player, ct);
        await _uow.SaveChangesAsync(ct);

        return player;
    }

    public Task<Player?> GetPlayerAsync(PlayerId id, CancellationToken ct)
        => _repo.GetByIdAsync(id, ct);

    public async Task<Player?> ClickAsync(PlayerId id, CancellationToken ct)
    {
        var player = await _repo.GetByIdAsync(id, ct);
        if (player is null) return null;

        player.Click();

        _repo.Update(player);
        await _uow.SaveChangesAsync(ct);

        return player;
    }

    public async Task<bool> DeletePlayerAsync(PlayerId id, CancellationToken ct)
    {
        var player = await _repo.GetByIdAsync(id, ct);
        if (player is null) return false;

        _repo.Remove(player);
        await _uow.SaveChangesAsync(ct);

        return true;
    }
}

