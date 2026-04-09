using System.Threading;
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

    public async Task<Result<Player>> CreatePlayerAsync(string name, string password, CancellationToken ct)
    {
        password = (password ?? "").Trim();

        if (password.Length < 6)
            return Result<Player>.Fail(Errors.InvalidPassword);

        var exists = await _repo.ExistsByNameAsync(name, ct);
        if (exists)
            return Result<Player>.Fail(Errors.PlayerNameAlreadyExists);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        if (!Player.TryCreate(name, passwordHash, out var player) || player is null)
            return Result<Player>.Fail(Errors.InvalidName);

        await _repo.AddAsync(player, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Player>.Ok(player);
    }

    public async Task<Result<Player>> LoginAsync(string name, string password, CancellationToken ct)
    {
        password = (password ?? "").Trim();

        var player = await _repo.GetByNameAsync(name, ct);
        if (player is null)
            return Result<Player>.Fail(Errors.InvalidCredentials);

        var isValidPassword = BCrypt.Net.BCrypt.Verify(password, player.PasswordHash);
        if (!isValidPassword)
            return Result<Player>.Fail(Errors.InvalidCredentials);

        return Result<Player>.Ok(player);
    }

    public async Task<Result<Player>> GetPlayerAsync(PlayerId id, CancellationToken ct)
    {
        var player = await _repo.GetByIdAsync(id, ct);
        if (player is null)
            return Result<Player>.Fail(Errors.PlayerNotFound);

        return Result<Player>.Ok(player);
    }

    public async Task<Result<Player>> UnlockFirstGameAsync(PlayerId id, CancellationToken ct)
    {
        var player = await _repo.GetByIdAsync(id, ct);
        if (player is null)
            return Result<Player>.Fail(Errors.PlayerNotFound);

        player.UnlockFirstGame();
        _repo.Update(player);
        await _uow.SaveChangesAsync(ct);

        return Result<Player>.Ok(player);
    }

    public async Task<Result> DeletePlayerAsync(PlayerId id, CancellationToken ct)
    {
        var player = await _repo.GetByIdAsync(id, ct);
        if (player is null)
            return Result.Fail(Errors.PlayerNotFound);

        _repo.Remove(player);
        await _uow.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
