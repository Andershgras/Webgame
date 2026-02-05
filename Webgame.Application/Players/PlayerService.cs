using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webgame.Application.Common;
using Webgame.Application.Persistence;
using Webgame.Domain.Players;

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

    public async Task<Result<Player>> CreatePlayerAsync(string name, CancellationToken ct)
    {
        if (!Player.TryCreate(name, out var player) || player is null)
            return Result<Player>.Fail(Errors.InvalidName);

        await _repo.AddAsync(player, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Player>.Ok(player);
    }

    public async Task<Result<Player>> GetPlayerAsync(PlayerId id, CancellationToken ct)
    {
        var player = await _repo.GetByIdAsync(id, ct);
        return player is null
            ? Result<Player>.Fail(Errors.PlayerNotFound)
            : Result<Player>.Ok(player);
    }

    public async Task<Result<Player>> ClickAsync(PlayerId id, CancellationToken ct)
    {
        var player = await _repo.GetByIdAsync(id, ct);
        if (player is null) return Result<Player>.Fail(Errors.PlayerNotFound);

        player.Click();
        _repo.Update(player);
        await _uow.SaveChangesAsync(ct);

        return Result<Player>.Ok(player);
    }

    public async Task<Result> DeletePlayerAsync(PlayerId id, CancellationToken ct)
    {
        var player = await _repo.GetByIdAsync(id, ct);
        if (player is null) return Result.Fail(Errors.PlayerNotFound);

        _repo.Remove(player);
        await _uow.SaveChangesAsync(ct);

        return Result.Ok();
    }
}

