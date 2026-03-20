using System;
using Webgame.Domain.Common;

namespace Webgame.Domain.Players;

public sealed class Player : Entity<PlayerId>
{
    private Player() : base() { } // EF Core

    public string Name { get; private set; }
    public string PasswordHash { get; private set; }
    public Stats Stats { get; private set; }

    // Offline progress
    public DateTime LastActiveUtc { get; private set; } = DateTime.UtcNow;
    private long _pendingOfflineCoins;
    private int _pendingOfflineSeconds;

    public Player(PlayerId id, string name, string passwordHash) : base(id)
    {
        Name = ValidateName(name);
        PasswordHash = ValidatePasswordHash(passwordHash);
        Stats = new Stats();
    }

    public void Rename(string newName)
    {
        Name = ValidateName(newName);
    }

    public void ChangePasswordHash(string newPasswordHash)
    {
        PasswordHash = ValidatePasswordHash(newPasswordHash);
    }

    private static string ValidateName(string name)
    {
        name = (name ?? "").Trim();

        if (name.Length is < 3 or > 20)
            throw new ArgumentException("Name must be between 3 and 20 characters.", nameof(name));

        return name;
    }

    private static string ValidatePasswordHash(string passwordHash)
    {
        passwordHash = (passwordHash ?? "").Trim();

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));

        return passwordHash;
    }

    public void Click()
    {
        Stats.RegisterClick();

        var coinsGained = Stats.ClickPower;
        Stats.AddCoins(coinsGained);

        var xpGained = Math.Max(1, (long)Math.Ceiling(Stats.ClickPower * 1.5));
        Stats.AddExperience(xpGained);
    }

    public static bool TryCreate(string name, string passwordHash, out Player? player)
    {
        name = (name ?? "").Trim();
        passwordHash = (passwordHash ?? "").Trim();

        if (name.Length is < 3 or > 20 || string.IsNullOrWhiteSpace(passwordHash))
        {
            player = null;
            return false;
        }

        player = new Player(PlayerId.New(), name, passwordHash);
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
        return 10L * Stats.ClickPowerLevel;
    }

    public bool TryUpgradeClickPower(out long cost)
    {
        cost = GetClickPowerUpgradeCost();

        if (!Stats.TrySpendCoins(cost))
            return false;

        Stats.UpgradeClickPower();

        var xpGained = Math.Max(1, cost / 10);
        Stats.AddExperience(xpGained);

        return true;
    }

    public long GetAutoClickerUpgradeCost()
    {
        return 100L * (Stats.AutoClickerLevel + 1);
    }

    public bool TryUpgradeAutoClicker(out long cost)
    {
        cost = GetAutoClickerUpgradeCost();
        if (!Stats.TrySpendCoins(cost)) return false;

        Stats.UpgradeAutoClicker();

        var xpGained = Math.Max(1, cost / 10);
        Stats.AddExperience(xpGained);

        return true;
    }

    public void Tick()
    {
        var coins = Stats.AutoCoinsPerTick;
        if (coins > 0)
            Stats.AddCoins(coins);

        var xpGained = Math.Max(0, (long)Math.Ceiling(Stats.AutoCoinsPerTick * 0.5));
        if (xpGained > 0)
            Stats.AddExperience(xpGained);
    }

    public void CalculateOfflineProgress(DateTime nowUtc)
    {
        if (nowUtc <= LastActiveUtc)
        {
            LastActiveUtc = nowUtc;
            return;
        }

        var elapsedSeconds = (int)Math.Floor((nowUtc - LastActiveUtc).TotalSeconds);
        var cappedSeconds = Math.Min(elapsedSeconds, Stats.OfflineCapSeconds);

        if (cappedSeconds > 0 && Stats.AutoCoinsPerTick > 0)
        {
            var coins = (long)cappedSeconds * Stats.AutoCoinsPerTick;

            Stats.AddCoins(coins);

            var xp = (long)Math.Ceiling(cappedSeconds * Stats.AutoCoinsPerTick * 0.5);
            if (xp > 0)
                Stats.AddExperience(xp);

            _pendingOfflineCoins = coins;
            _pendingOfflineSeconds = cappedSeconds;
        }

        LastActiveUtc = nowUtc;
    }

    public (long CoinsEarned, int SecondsApplied) ConsumeOfflineProgress()
    {
        var result = (_pendingOfflineCoins, _pendingOfflineSeconds);

        _pendingOfflineCoins = 0;
        _pendingOfflineSeconds = 0;

        return result;
    }

    public void Touch()
    {
        LastActiveUtc = DateTime.UtcNow;
    }

    public long GetOfflineCapUpgradeCost()
    {
        return 500L * (Stats.OfflineCapLevel + 1);
    }

    public bool TryUpgradeOfflineCap(out long cost)
    {
        cost = GetOfflineCapUpgradeCost();

        if (!Stats.TrySpendCoins(cost))
            return false;

        Stats.UpgradeOfflineCap();

        var xpGained = Math.Max(1, cost / 10);
        Stats.AddExperience(xpGained);

        return true;
    }
}