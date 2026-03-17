using Webgame.Application.Common;
using Webgame.Contracts.Leaderboards;

namespace Webgame.Application.Leaderboards;

public interface ILeaderboardQuery
{
    Task<Result<IReadOnlyList<LeaderboardEntry>>> GetTopAsync(int top, LeaderboardType type, CancellationToken ct);
    Task<Result<int>> GetRankAsync(Guid playerId, LeaderboardType type, CancellationToken ct);
}