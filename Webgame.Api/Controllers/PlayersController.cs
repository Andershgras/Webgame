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
    public sealed record MergeCoresRequest(Guid FirstCoreId, Guid SecondCoreId);

    // -------------------------
    // PUBLIC
    // -------------------------

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<PlayerResponse>> Create(
        [FromBody] CreatePlayerRequest request,
        CancellationToken ct)
    {
        var result = await _service.CreatePlayerAsync(request.Name, request.Password, ct);

        return ResultToHttp.ToActionResult<Player, PlayerResponse>(
            this,
            result,
            PlayerMappings.ToResponse,
            dto => CreatedAtAction(nameof(GetMe), null, dto));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
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
    // PROTECTED (/me)
    // -------------------------

    [HttpGet("me")]
    public async Task<ActionResult<PlayerResponse>> GetMe(CancellationToken ct)
    {
        var playerId = GetPlayerIdFromToken();
        if (playerId is null)
            return Unauthorized();

        var result = await _service.GetPlayerAsync(playerId.Value, ct);

        return ResultToHttp.ToActionResult<Player, PlayerResponse>(
            this,
            result,
            PlayerMappings.ToResponse,
            dto => Ok(dto));
    }

    [HttpPost("me/click")]
    public async Task<ActionResult<PlayerResponse>> Click(CancellationToken ct)
    {
        var playerId = GetPlayerIdFromToken();
        if (playerId is null)
            return Unauthorized();

        var result = await _service.ClickAsync(playerId.Value, ct);

        return ResultToHttp.ToActionResult<Player, PlayerResponse>(
            this,
            result,
            PlayerMappings.ToResponse,
            dto => Ok(dto));
    }

    [HttpPost("me/tick")]
    public async Task<ActionResult<PlayerResponse>> Tick(CancellationToken ct)
    {
        var playerId = GetPlayerIdFromToken();
        if (playerId is null)
            return Unauthorized();

        var result = await _service.TickAsync(playerId.Value, ct);

        return ResultToHttp.ToActionResult<Player, PlayerResponse>(
            this,
            result,
            PlayerMappings.ToResponse,
            dto => Ok(dto));
    }

    [HttpPost("me/cores/spawn")]
    public async Task<ActionResult<PlayerResponse>> SpawnCore(CancellationToken ct)
    {
        var playerId = GetPlayerIdFromToken();
        if (playerId is null)
            return Unauthorized();

        var result = await _service.SpawnCoreAsync(playerId.Value, ct);

        return ResultToHttp.ToActionResult<Player, PlayerResponse>(
            this,
            result,
            PlayerMappings.ToResponse,
            dto => Ok(dto));
    }

    [HttpPost("me/cores/merge")]
    public async Task<ActionResult<PlayerResponse>> MergeCores(
        [FromBody] MergeCoresRequest request,
        CancellationToken ct)
    {
        var playerId = GetPlayerIdFromToken();
        if (playerId is null)
            return Unauthorized();

        var result = await _service.MergeCoresAsync(
            playerId.Value,
            request.FirstCoreId,
            request.SecondCoreId,
            ct);

        return ResultToHttp.ToActionResult<Player, PlayerResponse>(
            this,
            result,
            PlayerMappings.ToResponse,
            dto => Ok(dto));
    }

    [HttpDelete("me")]
    public async Task<IActionResult> Delete(CancellationToken ct)
    {
        var playerId = GetPlayerIdFromToken();
        if (playerId is null)
            return Unauthorized();

        var result = await _service.DeletePlayerAsync(playerId.Value, ct);

        return ResultToHttp.ToActionResult(this, result, () => NoContent());
    }

    // -------------------------
    // HELPER
    // -------------------------

    private PlayerId? GetPlayerIdFromToken()
    {
        var claimValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub");

        if (!Guid.TryParse(claimValue, out var id))
            return null;

        return new PlayerId(id);
    }
}