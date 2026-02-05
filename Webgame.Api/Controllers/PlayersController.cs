using Microsoft.AspNetCore.Mvc;
using Webgame.Application.Players;
using Webgame.Domain.Players;

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
    public sealed record PlayerResponse(string Id, string Name, int Level, long Coins, int ClickPower);

    [HttpPost]
    public async Task<ActionResult<PlayerResponse>> Create([FromBody] CreatePlayerRequest request, CancellationToken ct)
    {
        var result = await _service.CreatePlayerAsync(request.Name, ct);

        if (!result.IsSuccess)
            return BadRequest(new { result.Error!.Code, result.Error.Message });

        var player = result.Value!;
        return CreatedAtAction(nameof(GetById), new { id = player.Id.Value }, ToResponse(player));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlayerResponse>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _service.GetPlayerAsync(new PlayerId(id), ct);

        if (!result.IsSuccess)
            return NotFound(new { result.Error!.Code, result.Error.Message });

        return Ok(ToResponse(result.Value!));
    }

    private static PlayerResponse ToResponse(Player p)
        => new(p.Id.Value.ToString(), p.Name, p.Stats.Level, p.Stats.Coins, p.Stats.ClickPower);

    [HttpPost("{id:guid}/click")]
    public async Task<ActionResult<PlayerResponse>> Click([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _service.ClickAsync(new PlayerId(id), ct);

        if (!result.IsSuccess)
            return NotFound(new { result.Error!.Code, result.Error.Message });

        return Ok(ToResponse(result.Value!));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _service.DeletePlayerAsync(new PlayerId(id), ct);

        if (!result.IsSuccess)
            return NotFound(new { result.Error!.Code, result.Error.Message });

        return NoContent();
    }
}

