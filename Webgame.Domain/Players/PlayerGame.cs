namespace Webgame.Domain.Players;

public sealed class PlayerGame
{
    private PlayerGame()
    {
    }

    public PlayerGame(Guid id, string name, int players, int revenue)
    {
        Id = id;
        Name = ValidateName(name);
        Players = ValidateResource(players, nameof(players));
        Revenue = ValidateResource(revenue, nameof(revenue));
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public int Players { get; private set; }
    public int Revenue { get; private set; }

    public static PlayerGame CreateFirstGame()
    {
        return new PlayerGame(Guid.NewGuid(), "Game #1", 0, 0);
    }

    private static string ValidateName(string name)
    {
        name = (name ?? "").Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Game name is required.", nameof(name));

        return name;
    }

    private static int ValidateResource(int value, string paramName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(paramName, "Resource values cannot be negative.");

        return value;
    }
}
