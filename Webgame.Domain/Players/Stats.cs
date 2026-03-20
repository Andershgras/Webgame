using System;

namespace Webgame.Domain.Players;

public sealed class Stats
{
    // EF Core
    public Stats() { }

    // Leveling
    public int Level { get; private set; } = 1;
    public long Experience { get; private set; } = 0;
    public long ExperienceToNextLevel { get; private set; } = CalculateExperienceToNextLevel(1);

    // Currency
    public long Coins { get; private set; } = 0;

    // Upgrades
    public int ClickPower { get; private set; } = 1;
    public int ClickPowerLevel { get; private set; } = 1;
    public int AutoClickerLevel { get; private set; } = 0;

    // Offline progress
    public int OfflineCapLevel { get; private set; } = 0;
    public int OfflineCapSeconds => 3600 + (OfflineCapLevel * 1800);

    // Helpers
    public int AutoCoinsPerTick => AutoClickerLevel;
    public int ClicksPerSecond => AutoClickerLevel;

    // Lifetime
    public long TotalClicks { get; private set; }
    public long TotalCoinsEarned { get; private set; }
    public long TotalCoinsSpent { get; private set; }

    public void UpgradeOfflineCap()
    {
        OfflineCapLevel++;
    }

    public void UpgradeAutoClicker()
    {
        AutoClickerLevel++;
    }

    public void UpgradeClickPower()
    {
        ClickPowerLevel++;
        ClickPower++;
    }

    public void RegisterClick()
    {
        TotalClicks++;
    }

    public void AddCoins(long amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        Coins += amount;
        TotalCoinsEarned += amount;
    }

    public bool TrySpendCoins(long amount)
    {
        if (amount < 0) return false;
        if (Coins < amount) return false;

        Coins -= amount;
        TotalCoinsSpent += amount;
        return true;
    }

    public void AddExperience(long amount)
    {
        if (amount <= 0)
            return;

        Experience += amount;

        while (Experience >= ExperienceToNextLevel)
        {
            Experience -= ExperienceToNextLevel;
            Level++;
            ExperienceToNextLevel = CalculateExperienceToNextLevel(Level);
        }
    }

    public static long CalculateExperienceToNextLevel(int level)
    {
        if (level < 1)
            level = 1;

        return (long)Math.Ceiling(100 * Math.Pow(level, 1.5));
    }
}