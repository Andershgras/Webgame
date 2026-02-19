using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webgame.Domain.Players;

public sealed class Stats
{
    public Stats() { } // EF Core
    // Leveling
    public int Level { get; private set; } = 1;
    // Currency
    public long Coins { get; private set; } = 0;
    // Upgrades
    public int ClickPower { get; private set; } = 1;
    public int ClickPowerLevel { get; private set; } = 1;
    public int CoinsPerClickLevel { get; private set; } = 0;
    public int AutoClickerLevel { get; private set; } = 0;
    // Helpers
    public int BonusCoinsPerClick => CoinsPerClickLevel; // 0,1,2,3...
    public int AutoCoinsPerTick => AutoClickerLevel; // 0,1,2,3...
    public void UpgradeCoinsPerClick() => CoinsPerClickLevel++;
    public void UpgradeAutoClicker() => AutoClickerLevel++;

    public void AddCoins(long amount)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Coins += amount;
    }

    public bool TrySpendCoins(long amount)
    {
        if (amount < 0) return false;
        if (Coins < amount) return false;

        Coins -= amount;
        return true;
    }
    public void UpgradeClickPower()
    {
        ClickPowerLevel++;
        ClickPower++;
    }
}

