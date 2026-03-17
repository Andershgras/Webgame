using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webgame.Application.Common;
using Webgame.Contracts.Leaderboards;

namespace Webgame.Application.Leaderboards;

public interface ILeaderboardQuery
{
    Task<Result<IReadOnlyList<LeaderboardEntry>>> GetTopAsync(int top, string type, CancellationToken ct);
}

