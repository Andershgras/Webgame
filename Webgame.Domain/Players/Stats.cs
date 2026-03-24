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

    // Main run currency
    public long Energy { get; private set; } = 0;

    // Permanent currencies
    public long StellarEnergy { get; private set; } = 0;
    public long StormEnergy { get; private set; } = 0;

    // Legacy compatibility
    public long Coins => Energy;

    // Temporary legacy clicker upgrades
    public int ClickPower { get; private set; } = 1;
    public int ClickPowerLevel { get; private set; } = 1;
    public int AutoClickerLevel { get; private set; } = 0;

    // Offline progress
    public int OfflineCapLevel { get; private set; } = 0;
    public int OfflineCapSeconds => 3600 + (OfflineCapLevel * 1800);

    // Legacy helpers
    public int AutoCoinsPerTick => AutoClickerLevel;
    public int ClicksPerSecond => AutoClickerLevel;

    // Lifetime
    public long TotalClicks { get; private set; }
    public long TotalEnergyEarned { get; private set; }
    public long TotalEnergySpent { get; private set; }
    public long TotalMerges { get; private set; }

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

    public void RegisterMerge(long xpGained, long stormEnergyReward = 0)
    {
        TotalMerges++;

        if (xpGained > 0)
            AddExperience(xpGained);

        if (stormEnergyReward > 0)
            AddStormEnergy(stormEnergyReward);
    }

    public void AddEnergy(long amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        Energy += amount;
        TotalEnergyEarned += amount;
    }

    public bool TrySpendEnergy(long amount)
    {
        if (amount < 0) return false;
        if (Energy < amount) return false;

        Energy -= amount;
        TotalEnergySpent += amount;
        return true;
    }

    public void AddStellarEnergy(long amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        StellarEnergy += amount;
    }

    public bool TrySpendStellarEnergy(long amount)
    {
        if (amount < 0) return false;
        if (StellarEnergy < amount) return false;

        StellarEnergy -= amount;
        return true;
    }

    public void AddStormEnergy(long amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        StormEnergy += amount;
    }

    public bool TrySpendStormEnergy(long amount)
    {
        if (amount < 0) return false;
        if (StormEnergy < amount) return false;

        StormEnergy -= amount;
        return true;
    }

    // Compatibility wrappers so existing code can still work while we migrate
    public void AddCoins(long amount) => AddEnergy(amount);
    public bool TrySpendCoins(long amount) => TrySpendEnergy(amount);

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