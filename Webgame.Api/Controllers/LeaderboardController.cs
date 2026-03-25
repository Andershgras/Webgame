using Microsoft.AspNetCore.Mvc;
using Webgame.Api.Common;
using Webgame.Application.Leaderboards;
using Webgame.Contracts.Leaderboards;

namespace Webgame.Api.Controllers;

[ApiController]
[Route("api/leaderboard")]
public sealed class LeaderboardController : ControllerBase
{
    private readonly ILeaderboardQuery _query;

    public LeaderboardController(ILeaderboardQuery query)
    {
        _query = query;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LeaderboardEntry>>> Get(
        [FromQuery] int top = 10,
        [FromQuery] LeaderboardType type = LeaderboardType.TotalEnergyEarned,
        CancellationToken ct = default)
    {
        var result = await _query.GetTopAsync(top, type, ct);

        return ResultToHttp.ToActionResult<IReadOnlyList<LeaderboardEntry>, IReadOnlyList<LeaderboardEntry>>(
            this,
            result,
            x => x,
            dto => Ok(dto));
    }

    [HttpGet("rank")]
    public async Task<ActionResult<int>> GetRank(
        [FromQuery] Guid playerId,
        [FromQuery] LeaderboardType type = LeaderboardType.TotalEnergyEarned,
        CancellationToken ct = default)
    {
        var result = await _query.GetRankAsync(playerId, type, ct);

        return ResultToHttp.ToActionResult<int, int>(
            this,
            result,
            x => x,
            dto => Ok(dto));
    }
}