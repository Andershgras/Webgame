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

    public async Task<Result<IReadOnlyList<LeaderboardEntry>>> GetTopAsync(int top, string type, CancellationToken ct)
    {
        if (top is < 1 or > 100)
            return Result<IReadOnlyList<LeaderboardEntry>>.Fail(Errors.InvalidTop);

        type = string.IsNullOrWhiteSpace(type) ? "Coins" : type.Trim();

        var query = _db.Players
            .AsNoTracking()
            .AsQueryable();

        query = type switch
        {
            "Coins" => query
                .OrderByDescending(p => p.Stats.Coins)
                .ThenByDescending(p => p.Stats.Level)
                .ThenByDescending(p => p.Stats.ClickPower),

            "ClickPower" => query
                .OrderByDescending(p => p.Stats.ClickPower)
                .ThenByDescending(p => p.Stats.Level)
                .ThenByDescending(p => p.Stats.Coins),

            "Level" => query
                .OrderByDescending(p => p.Stats.Level)
                .ThenByDescending(p => p.Stats.Coins)
                .ThenByDescending(p => p.Stats.ClickPower),

            _ => query
                .OrderByDescending(p => p.Stats.Coins)
                .ThenByDescending(p => p.Stats.Level)
                .ThenByDescending(p => p.Stats.ClickPower)
        };

        var list = await query
            .Take(top)
            .Select(p => new LeaderboardEntry(
                p.Id.Value,
                p.Name,
                p.Stats.Level,
                p.Stats.Coins,
                p.Stats.ClickPower))
            .ToListAsync(ct);

        return Result<IReadOnlyList<LeaderboardEntry>>.Ok(list);
    }
}