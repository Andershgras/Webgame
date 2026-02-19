using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webgame.Domain.Common;

namespace Webgame.Domain.Players;
public sealed class Player : Entity<PlayerId>
{
    private Player() : base() { } // EF Core
    public string Name { get; private set; }
    public Stats Stats { get; private set; }

    public Player(PlayerId id, string name) : base(id)
    {
        Name = ValidateName(name);
        Stats = new Stats();
    }

    public void Rename(string newName)
    {
        Name = ValidateName(newName);
    }

    private static string ValidateName(string name)
    {
        name = (name ?? "").Trim();
        if (name.Length is < 3 or > 20)
            throw new ArgumentException("Name must be between 3 and 20 characters.", nameof(name));

        return name;
    }
    public void Click()
    {
        Stats.RegisterClick();

        var coinsGained = Stats.ClickPower + Stats.BonusCoinsPerClick;
        Stats.AddCoins(Stats.ClickPower);
    }
    public static bool TryCreate(string name, out Player? player)
    {
        name = (name ?? "").Trim();
        if (name.Length is < 3 or > 20)
        {
            player = null;
            return false;
        }

        player = new Player(PlayerId.New(), name);
        return true;
    }

    public bool TryRename(string newName)
    {
        newName = (newName ?? "").Trim();
        if (newName.Length is < 3 or > 20) return false;

        Name = newName;
        return true;
    }
    public long GetClickPowerUpgradeCost()
    {
        // Eksempel: 10, 20, 30, 40...
        return 10L * Stats.ClickPowerLevel;
    }
    public bool TryUpgradeClickPower(out long cost)
    {
        cost = GetClickPowerUpgradeCost();

        if (!Stats.TrySpendCoins(cost))
            return false;

        Stats.UpgradeClickPower();
        return true;
    }
    public long GetCoinsPerClickUpgradeCost()
    {
        // 25, 50, 75, 100...
        return 25L * (Stats.CoinsPerClickLevel + 1);
    }

    public bool TryUpgradeCoinsPerClick(out long cost)
    {
        cost = GetCoinsPerClickUpgradeCost();
        if (!Stats.TrySpendCoins(cost)) return false;

        Stats.UpgradeCoinsPerClick();
        return true;
    }

    public long GetAutoClickerUpgradeCost()
    {
        // 100, 200, 300...
        return 100L * (Stats.AutoClickerLevel + 1);
    }

    public bool TryUpgradeAutoClicker(out long cost)
    {
        cost = GetAutoClickerUpgradeCost();
        if (!Stats.TrySpendCoins(cost)) return false;

        Stats.UpgradeAutoClicker();
        return true;
    }
    public void Tick()
    {
        var coins = Stats.AutoCoinsPerTick;
        if (coins > 0)
            Stats.AddCoins(coins);
    }
}
