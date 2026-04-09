namespace Webgame.Domain.Players;

public sealed class Stats
{
    private Stats()
    {
    }

    public Stats(int currency)
    {
        Currency = ValidateCurrency(currency);
    }

    public int Currency { get; private set; }

    public void SetCurrency(int currency)
    {
        Currency = ValidateCurrency(currency);
    }

    private static int ValidateCurrency(int currency)
    {
        if (currency < 0)
            throw new ArgumentOutOfRangeException(nameof(currency), "Currency cannot be negative.");

        return currency;
    }
}
