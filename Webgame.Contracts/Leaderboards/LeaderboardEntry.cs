using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webgame.Contracts.Leaderboards;

public sealed record LeaderboardEntry(
    Guid PlayerId,
    string Name,
    long Value
);
