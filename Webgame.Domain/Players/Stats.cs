using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webgame.Domain.Players;

public sealed class Stats
{
    public Stats() { } // EF Core
    public int Level { get; private set; } = 1;
    public long Coins { get; private set; } = 0;
    public int ClickPower { get; private set; } = 1;
    public int ClickPowerLevel { get; private set; } = 1;

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

