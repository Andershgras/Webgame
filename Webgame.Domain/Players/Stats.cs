using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webgame.Domain.Players;

public sealed class Stats
{
    // EF Core
    public Stats() { }

    // Leveling
    public int Level { get; private set; } = 1;

    // Currency
    public long Coins { get; private set; } = 0;

    // Upgrades
    public int ClickPower { get; private set; } = 1;
    public int ClickPowerLevel { get; private set; } = 1;
    public int AutoClickerLevel { get; private set; } = 0;

    // Offline progress
    public int OfflineCapLevel { get; private set; } = 0;

    public int OfflineCapSeconds => 3600 + (OfflineCapLevel * 1800);

    public void UpgradeOfflineCap()
    {
        OfflineCapLevel++;
    }

    // Helpers
    public int AutoCoinsPerTick => AutoClickerLevel;
    public void UpgradeAutoClicker() => AutoClickerLevel++;

    // Lifetime
    public long TotalClicks { get; private set; }
    public long TotalCoinsEarned { get; private set; }
    public long TotalCoinsSpent { get; private set; }

    public void AddCoins(long amount)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));

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

    public void UpgradeClickPower()
    {
        ClickPowerLevel++;
        ClickPower++;
    }

    public void RegisterClick()
    {
        TotalClicks++;
    }
}