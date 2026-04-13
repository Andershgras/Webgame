namespace Webgame.Domain.Players;

public static class GameDefinitions
{
    public const string FirstGameKey = "game-1";

    private static readonly IReadOnlyDictionary<string, GameDefinition> Definitions =
        new Dictionary<string, GameDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            [FirstGameKey] = new(
                FirstGameKey,
                "Game #1",
                StartingPlayers: 0,
                StartingRevenue: 0)
        };

    public static GameDefinition Get(string key)
    {
        key = (key ?? "").Trim();

        if (!Definitions.TryGetValue(key, out var definition))
            throw new ArgumentException($"Unknown game definition key '{key}'.", nameof(key));

        return definition;
    }
}
