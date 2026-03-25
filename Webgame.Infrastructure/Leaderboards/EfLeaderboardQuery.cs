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
                p.Stats.TotalEnergyEarned,
                p.Stats.TotalEnergySpent,
                p.Stats.TotalMerges))
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
                p.Stats.Level,
                p.Stats.TotalEnergyEarned,
                p.Stats.TotalEnergySpent,
                p.Stats.TotalMerges))
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
            LeaderboardType.TotalEnergyEarned => players
                .OrderByDescending(p => p.TotalEnergyEarned)
                .ThenByDescending(p => p.TotalMerges)
                .ThenByDescending(p => p.Level)
                .ThenByDescending(p => p.TotalEnergySpent)
                .ThenBy(p => p.PlayerId),

            LeaderboardType.TotalEnergySpent => players
                .OrderByDescending(p => p.TotalEnergySpent)
                .ThenByDescending(p => p.TotalEnergyEarned)
                .ThenByDescending(p => p.TotalMerges)
                .ThenByDescending(p => p.Level)
                .ThenBy(p => p.PlayerId),

            LeaderboardType.TotalMerges => players
                .OrderByDescending(p => p.TotalMerges)
                .ThenByDescending(p => p.TotalEnergyEarned)
                .ThenByDescending(p => p.Level)
                .ThenByDescending(p => p.TotalEnergySpent)
                .ThenBy(p => p.PlayerId),

            LeaderboardType.Level => players
                .OrderByDescending(p => p.Level)
                .ThenByDescending(p => p.TotalEnergyEarned)
                .ThenByDescending(p => p.TotalMerges)
                .ThenByDescending(p => p.TotalEnergySpent)
                .ThenBy(p => p.PlayerId),

            _ => players
                .OrderByDescending(p => p.TotalEnergyEarned)
                .ThenByDescending(p => p.TotalMerges)
                .ThenByDescending(p => p.Level)
                .ThenByDescending(p => p.TotalEnergySpent)
                .ThenBy(p => p.PlayerId)
        };
    }

    private static long GetValueByType(LeaderboardRow row, LeaderboardType type)
    {
        return type switch
        {
            LeaderboardType.TotalEnergyEarned => row.TotalEnergyEarned,
            LeaderboardType.TotalEnergySpent => row.TotalEnergySpent,
            LeaderboardType.TotalMerges => row.TotalMerges,
            LeaderboardType.Level => row.Level,
            _ => row.TotalEnergyEarned
        };
    }

    private sealed record LeaderboardRow(
        Guid PlayerId,
        string Name,
        int Level,
        long TotalEnergyEarned,
        long TotalEnergySpent,
        long TotalMerges);
}