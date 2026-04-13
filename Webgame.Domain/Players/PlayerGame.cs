namespace Webgame.Domain.Players;

public sealed class PlayerGame
{
    private PlayerGame()
    {
    }

    public PlayerGame(Guid id, string gameKey, int players, int revenue)
    {
        Id = id;
        GameKey = ValidateGameKey(gameKey);
        Players = ValidateResource(players, nameof(players));
        Revenue = ValidateResource(revenue, nameof(revenue));
    }

    public Guid Id { get; private set; }
    public string GameKey { get; private set; } = null!;
    public string Name => GameDefinitions.Get(GameKey).Name;
    public int Players { get; private set; }
    public int Revenue { get; private set; }

    public static PlayerGame Create(string gameKey)
    {
        var definition = GameDefinitions.Get(gameKey);
        return new PlayerGame(Guid.NewGuid(), definition.Key, definition.StartingPlayers, definition.StartingRevenue);
    }

    private static string ValidateGameKey(string gameKey)
    {
        return GameDefinitions.Get(gameKey).Key;
    }

    private static int ValidateResource(int value, string paramName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(paramName, "Resource values cannot be negative.");

        return value;
    }
}
