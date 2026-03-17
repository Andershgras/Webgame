using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Webgame.Application.Common;
using Webgame.Contracts.Leaderboards;
using Webgame.Application.Leaderboards;
using Webgame.Infrastructure.Persistence;

namespace Webgame.Infrastructure.Leaderboards;

public sealed class EfLeaderboardQuery : ILeaderboardQuery
{
    private readonly WebgameDbContext _db;

    public EfLeaderboardQuery(WebgameDbContext db)
    {
        _db = db;
    }
    // Get top N players sorted by Level, then Coins, then ClickPower
    public async Task<Result<IReadOnlyList<LeaderboardEntry>>> GetTopAsync(int top, CancellationToken ct)
    {
        if (top is < 1 or > 100)
            return Result<IReadOnlyList<LeaderboardEntry>>.Fail(Errors.InvalidTop);

        var list = await _db.Players
            .AsNoTracking()
            .OrderByDescending(p => p.Stats.Level)
            .ThenByDescending(p => p.Stats.Coins)
            .ThenByDescending(p => p.Stats.ClickPower)
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

