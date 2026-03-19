using System;

namespace Webgame.Contracts.Players;

public sealed record LoginResponse(
    string Token,
    DateTime ExpiresAtUtc,
    PlayerResponse Player);