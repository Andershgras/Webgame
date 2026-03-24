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
                p.Stats.TotalClicks,
                p.Stats.TotalCoinsEarned,
                p.Stats.TotalCoinsSpent))
            .ToListAsync(ct);

        var list = SortPlayers(players, type)
            .Take(top)
            .Select(p => new LeaderboardEntry(
                p.PlayerId,
                p.Name,
                GetValueByType(p, type)))
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
                p.Stats.TotalClicks,
                p.Stats.TotalCoinsEarned,
                p.Stats.TotalCoinsSpent))
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
            LeaderboardType.TotalClicks => players
                .OrderByDescending(p => p.TotalClicks)
                .ThenByDescending(p => p.TotalCoinsEarned)
                .ThenByDescending(p => p.TotalCoinsSpent)
                .ThenBy(p => p.PlayerId),

            LeaderboardType.TotalCoinsEarned => players
                .OrderByDescending(p => p.TotalCoinsEarned)
                .ThenByDescending(p => p.TotalClicks)
                .ThenByDescending(p => p.TotalCoinsSpent)
                .ThenBy(p => p.PlayerId),

            LeaderboardType.TotalCoinsSpent => players
                .OrderByDescending(p => p.TotalCoinsSpent)
                .ThenByDescending(p => p.TotalCoinsEarned)
                .ThenByDescending(p => p.TotalClicks)
                .ThenBy(p => p.PlayerId),

            _ => players
                .OrderByDescending(p => p.TotalClicks)
                .ThenByDescending(p => p.TotalCoinsEarned)
                .ThenByDescending(p => p.TotalCoinsSpent)
                .ThenBy(p => p.PlayerId)
        };
    }

    private static long GetValueByType(LeaderboardRow row, LeaderboardType type)
    {
        return type switch
        {
            LeaderboardType.TotalClicks => row.TotalClicks,
            LeaderboardType.TotalCoinsEarned => row.TotalCoinsEarned,
            LeaderboardType.TotalCoinsSpent => row.TotalCoinsSpent,
            _ => row.TotalClicks
        };
    }

    private sealed record LeaderboardRow(
        Guid PlayerId,
        string Name,
        long TotalClicks,
        long TotalCoinsEarned,
        long TotalCoinsSpent);
}