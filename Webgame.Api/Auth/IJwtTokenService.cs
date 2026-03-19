using Webgame.Domain.Players;

namespace Webgame.Api.Auth;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) CreateToken(Player player);
}