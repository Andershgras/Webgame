using System;
using Webgame.Domain.Common;

namespace Webgame.Domain.Players;

public sealed class Player : Entity<PlayerId>
{
    private Player() : base() { } // EF Core

    public string Name { get; private set; }
    public string PasswordHash { get; private set; }
    public Stats Stats { get; private set; }
    public Board Board { get; private set; }

    // Offline progress
    public DateTime LastActiveUtc { get; private set; } = DateTime.UtcNow;
    private long _pendingOfflineEnergy;
    private int _pendingOfflineSeconds;

    public Player(PlayerId id, string name, string passwordHash) : base(id)
    {
        Name = ValidateName(name);
        PasswordHash = ValidatePasswordHash(passwordHash);
        Stats = new Stats();
        Board = new Board();
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

        var energyGained = Stats.ClickPower;
        Stats.AddEnergy(energyGained);
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

        if (!Stats.TrySpendEnergy(cost))
            return false;

        Stats.UpgradeClickPower();
        return true;
    }

    public long GetAutoClickerUpgradeCost()
    {
        return 100L * (Stats.AutoClickerLevel + 1);
    }

    public bool TryUpgradeAutoClicker(out long cost)
    {
        cost = GetAutoClickerUpgradeCost();

        if (!Stats.TrySpendEnergy(cost))
            return false;

        Stats.UpgradeAutoClicker();
        return true;
    }

    public long GetOfflineCapUpgradeCost()
    {
        return 500L * (Stats.OfflineCapLevel + 1);
    }

    public bool TryUpgradeOfflineCap(out long cost)
    {
        cost = GetOfflineCapUpgradeCost();

        if (!Stats.TrySpendEnergy(cost))
            return false;

        Stats.UpgradeOfflineCap();
        return true;
    }

    // -------------------------
    // CORE SYSTEM
    // -------------------------

    public bool TrySpawnCore()
    {
        return Board.TrySpawnTier1Core(out _);
    }

    public bool TryMergeCores(Guid firstCoreId, Guid secondCoreId, out long xpGained)
    {
        xpGained = 0;

        if (!Board.TryMerge(firstCoreId, secondCoreId, out var merged) || merged is null)
            return false;

        xpGained = merged.Tier;

        // XP is awarded only on merge
        Stats.RegisterMerge(xpGained, 0);
        return true;
    }

    public long GetProductionPerSecond()
    {
        var baseProduction = Board.GetProductionPerSecond();

        if (baseProduction <= 0)
            return 0;

        // Future: Stellar / Star boosts go here
        return baseProduction;
    }

    public void Tick()
    {
        var energy = GetProductionPerSecond();

        if (energy > 0)
            Stats.AddEnergy(energy);
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

        var productionPerSecond = GetProductionPerSecond();

        if (cappedSeconds > 0 && productionPerSecond > 0)
        {
            var energyEarned = (long)cappedSeconds * productionPerSecond;

            Stats.AddEnergy(energyEarned);

            _pendingOfflineEnergy = energyEarned;
            _pendingOfflineSeconds = cappedSeconds;
        }

        LastActiveUtc = nowUtc;
    }

    public (long EnergyEarned, int SecondsApplied) ConsumeOfflineProgress()
    {
        var result = (_pendingOfflineEnergy, _pendingOfflineSeconds);

        _pendingOfflineEnergy = 0;
        _pendingOfflineSeconds = 0;

        return result;
    }

    public void Touch()
    {
        LastActiveUtc = DateTime.UtcNow;
    }
}