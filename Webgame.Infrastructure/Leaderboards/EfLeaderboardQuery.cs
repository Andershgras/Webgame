using Microsoft.EntityFrameworkCore;
using Webgame.Application.Common;
using Webgame.Application.Leaderboards;
using Webgame.Contracts.Leaderboards;
using Webgame.Infrastructure.Persistence;

namespace Webgame.Infrastructure.Leaderboards;

public sealed class EfLeaderboardQuery : ILeaderboardQuery
{
    private readonly WebgameDbContext _db;

    public EfLeaderboardQuery(WebgameDbContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyList<LeaderboardEntry>>> GetTopAsync(int top, LeaderboardType type, CancellationToken ct)
    {
        if (top is < 1 or > 100)
            return Result<IReadOnlyList<LeaderboardEntry>>.Fail(Errors.InvalidTop);

        var players = await _db.Players
            .AsNoTracking()
            .Select(p => new LeaderboardRow(
                p.Id.Value,
                p.Name,
                p.Stats.Level,
                p.Stats.Coins,
                p.Stats.ClickPower))
            .ToListAsync(ct);

        var list = SortPlayers(players, type)
            .Take(top)
            .Select(p => new LeaderboardEntry(
                p.PlayerId,
                p.Name,
                p.Level,
                p.Coins,
                p.ClickPower))
            .ToList();

        return Result<IReadOnlyList<LeaderboardEntry>>.Ok(list);
    }

    public async Task<Result<int>> GetRankAsync(Guid playerId, LeaderboardType type, CancellationToken ct)
    {
        var players = await _db.Players
            .AsNoTracking()
            .Select(p => new LeaderboardRow(
                p.Id.Value,
                p.Name,
                p.Stats.Level,
                p.Stats.Coins,
                p.Stats.ClickPower))
            .ToListAsync(ct);

        var ordered = SortPlayers(players, type).ToList();

        var index = ordered.FindIndex(p => p.PlayerId == playerId);
        if (index < 0)
            return Result<int>.Fail(Errors.PlayerNotFound);

        return Result<int>.Ok(index + 1);
    }

    private static IEnumerable<LeaderboardRow> SortPlayers(IEnumerable<LeaderboardRow> players, LeaderboardType type)
    {
        return type switch
        {
            LeaderboardType.Coins => players
                .OrderByDescending(p => p.Coins)
                .ThenByDescending(p => p.Level)
                .ThenByDescending(p => p.ClickPower)
                .ThenBy(p => p.PlayerId),

            LeaderboardType.ClickPower => players
                .OrderByDescending(p => p.ClickPower)
                .ThenByDescending(p => p.Level)
                .ThenByDescending(p => p.Coins)
                .ThenBy(p => p.PlayerId),

            LeaderboardType.Level => players
                .OrderByDescending(p => p.Level)
                .ThenByDescending(p => p.Coins)
                .ThenByDescending(p => p.ClickPower)
                .ThenBy(p => p.PlayerId),

            _ => players
                .OrderByDescending(p => p.Coins)
                .ThenByDescending(p => p.Level)
                .ThenByDescending(p => p.ClickPower)
                .ThenBy(p => p.PlayerId)
        };
    }

    private sealed record LeaderboardRow(
        Guid PlayerId,
        string Name,
        int Level,
        long Coins,
        int ClickPower);
}