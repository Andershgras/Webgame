using System;

namespace Webgame.Contracts.Players;

public sealed record PlayerResponse(
    Guid Id,
    string Name,
    int Currency
);
