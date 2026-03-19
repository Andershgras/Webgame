using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Webgame.Api.Auth;
using Webgame.Api.Common;
using Webgame.Application.Common;
using Webgame.Application.Players;
using Webgame.Contracts.Players;
using Webgame.Domain.Players;

namespace Webgame.Api.Controllers;

[ApiController]
[Route("api/players")]
[Authorize]
public sealed class PlayersController : ControllerBase
{
    private readonly PlayerService _service;
    private readonly IJwtTokenService _jwtTokenService;

    public PlayersController(PlayerService service, IJwtTokenService jwtTokenService)
    {
        _service = service;
        _jwtTokenService = jwtTokenService;
    }

    public sealed record CreatePlayerRequest(string Name, string Password);
    public sealed record LoginRequest(string Name, string Password);

    // -------------------------
    // PUBLIC (NO AUTH REQUIRED)
    // -------------------------

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<PlayerResponse>> Create([FromBody] CreatePlayerRequest request, CancellationToken ct)
    {
        var result = await _service.CreatePlayerAsync(request.Name, request.Password, ct);

        return ResultToHttp.ToActionResult<Player, PlayerResponse>(
            this,
            result,
            PlayerMappings.ToResponse,
            dto => CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _service.LoginAsync(request.Name, request.Password, ct);

        if (!result.IsSuccess || result.Value is null)
        {
            return Unauthorized(new
            {
                code = Errors.InvalidCredentials.Code,
                message = Errors.InvalidCredentials.Message
            });
        }

        var player = result.Value;
        var playerDto = PlayerMappings.ToResponse(player);
        var (token, expiresAtUtc) = _jwtTokenService.CreateToken(player);

        return Ok(new LoginResponse(token, expiresAtUtc, playerDto));
    }

    // -------------------------
    // PROTECTED (AUTH REQUIRED)
    // -------------------------

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlayerResponse>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        if (!IsAuthorizedForPlayer(id))
            return Forbid();

        var result = await _service.GetPlayerAsync(new PlayerId(id), ct);

        return ResultToHttp.ToActionResult<Player, PlayerResponse>(
            this,
            result,
            PlayerMappings.ToResponse,
            dto => Ok(dto));
    }

    [HttpPost("{id:guid}/click")]
    public async Task<ActionResult<PlayerResponse>> Click([FromRoute] Guid id, CancellationToken ct)
    {
        if (!IsAuthorizedForPlayer(id))
            return Forbid();

        var result = await _service.ClickAsync(new PlayerId(id), ct);

        return ResultToHttp.ToActionResult<Player, PlayerResponse>(
            this,
            result,
            PlayerMappings.ToResponse,
            dto => Ok(dto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        if (!IsAuthorizedForPlayer(id))
            return Forbid();

        var result = await _service.DeletePlayerAsync(new PlayerId(id), ct);

        return ResultToHttp.ToActionResult(this, result, () => NoContent());
    }

    [HttpPost("{id:guid}/tick")]
    public async Task<ActionResult<PlayerResponse>> Tick([FromRoute] Guid id, CancellationToken ct)
    {
        if (!IsAuthorizedForPlayer(id))
            return Forbid();

        var result = await _service.TickAsync(new PlayerId(id), ct);

        return ResultToHttp.ToActionResult<Player, PlayerResponse>(
            this,
            result,
            PlayerMappings.ToResponse,
            dto => Ok(dto));
    }

    // -------------------------
    // HELPERS
    // -------------------------

    private bool IsAuthorizedForPlayer(Guid routePlayerId)
    {
        var claimValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub");

        return Guid.TryParse(claimValue, out var tokenPlayerId) && tokenPlayerId == routePlayerId;
    }
}