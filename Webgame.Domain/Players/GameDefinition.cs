namespace Webgame.Domain.Players;

public sealed record GameDefinition(
    string Key,
    string Name,
    int StartingPlayers,
    int StartingRevenue
);
