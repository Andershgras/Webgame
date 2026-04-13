using System;

namespace Webgame.Contracts.Players;

public sealed record PlayerGameResponse(
    Guid Id,
    string GameKey,
    string Name,
    int Players,
    int Revenue
);
