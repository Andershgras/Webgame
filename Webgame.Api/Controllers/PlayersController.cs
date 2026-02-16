using Microsoft.AspNetCore.Mvc;
using Webgame.Application.Players;
using Webgame.Domain.Players;
using Webgame.Api.Common;

namespace Webgame.Api.Controllers;

[ApiController]
[Route("api/players")]
public sealed class PlayersController : ControllerBase
{
    private readonly PlayerService _service;

    public PlayersController(PlayerService service)
    {
        _service = service;
    }

    public sealed record CreatePlayerRequest(string Name);
    public sealed record PlayerResponse(
        Guid Id,
        string Name,
        int Level,
        long Coins,
        int ClickPower,
        int ClickPowerLevel
    );
    [HttpPost]
    public async Task<ActionResult<PlayerResponse>> Create([FromBody] CreatePlayerRequest request, CancellationToken ct)
    {
        var result = await _service.CreatePlayerAsync(request.Name, ct);

        return ResultToHttp.ToActionResult<Player, PlayerResponse>(
            this,
            result,
            ToResponse,
            dto => CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto));
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlayerResponse>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _service.GetPlayerAsync(new PlayerId(id), ct);

        return ResultToHttp.ToActionResult<Player, PlayerResponse>(
            this,
            result,
            ToResponse,
            dto => Ok(dto));
    }
    private static PlayerResponse ToResponse(Player p)
        => new(
            p.Id.Value,
            p.Name,
            p.Stats.Level,
            p.Stats.Coins,
            p.Stats.ClickPower,
            p.Stats.ClickPowerLevel
        );

    [HttpPost("{id:guid}/click")]
    public async Task<ActionResult<PlayerResponse>> Click([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _service.ClickAsync(new PlayerId(id), ct);

        return ResultToHttp.ToActionResult<Player, PlayerResponse>(
            this,
            result,
            ToResponse,
            dto => Ok(dto));
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _service.DeletePlayerAsync(new PlayerId(id), ct);

        return ResultToHttp.ToActionResult(this, result, () => NoContent());
    }
    [HttpPost("{id:guid}/upgrades/click-power")]
    public async Task<ActionResult<PlayerResponse>> UpgradeClickPower([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _service.UpgradeClickPowerAsync(new PlayerId(id), ct);

        return ResultToHttp.ToActionResult<Player, PlayerResponse>(
            this,
            result,
            ToResponse,
            dto => Ok(dto));
    }
}

