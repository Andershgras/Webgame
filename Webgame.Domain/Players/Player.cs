using System;
using Webgame.Domain.Common;

namespace Webgame.Domain.Players;

public sealed class Player : Entity<PlayerId>
{
    private const double BaseSpawnIntervalSeconds = 5.0;
    private const double FasterCoresReductionPerLevel = 0.1;
    private const double MinSpawnIntervalSeconds = 1.0;

    private const double BaseStellarEnergyMergeChance = 0.01;
    private const double MoreStellarEnergyChancePerLevel = 0.001;

    private const double StellarSpawnStellarEnergyChancePerLevel = 0.02;
    private const double FasterLevelUpXpBonusPerLevel = 0.10;

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
        if (newName.Length is < 3 or > 20)
            return false;

        Name = newName;
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

    public long GetFasterCoresUpgradeCost()
    {
        return 25L * (Stats.FasterCoresLevel + 1);
    }

    public bool TryUpgradeFasterCores(out long cost)
    {
        cost = GetFasterCoresUpgradeCost();

        if (Stats.FasterCoresLevel >= Stats.MaxFasterCoresLevel)
            return false;

        if (!Stats.TrySpendEnergy(cost))
            return false;

        return Stats.TryUpgradeFasterCores();
    }

    public long GetBetterCoresUpgradeCost()
    {
        return 100L * (Stats.BetterCoresLevel + 1);
    }

    public bool TryUpgradeBetterCores(out long cost)
    {
        cost = GetBetterCoresUpgradeCost();

        if (Stats.BetterCoresLevel >= Stats.MaxBetterCoresLevel)
            return false;

        if (!Stats.TrySpendEnergy(cost))
            return false;

        return Stats.TryUpgradeBetterCores();
    }

    public long GetBetterCores2UpgradeCost()
    {
        return 150L * (Stats.BetterCores2Level + 1);
    }

    public bool TryUpgradeBetterCores2(out long cost)
    {
        cost = GetBetterCores2UpgradeCost();

        if (Stats.BetterCores2Level >= Stats.MaxBetterCores2Level)
            return false;

        if (!Stats.TrySpendEnergy(cost))
            return false;

        return Stats.TryUpgradeBetterCores2();
    }

    public long GetOfflineProductionUpgradeCost()
    {
        return 75L * (Stats.OfflineProductionLevel + 1);
    }

    public bool TryUpgradeOfflineProduction(out long cost)
    {
        cost = GetOfflineProductionUpgradeCost();

        if (Stats.OfflineProductionLevel >= Stats.MaxOfflineProductionLevel)
            return false;

        if (!Stats.TrySpendEnergy(cost))
            return false;

        return Stats.TryUpgradeOfflineProduction();
    }

    public long GetMoreStellarEnergyUpgradeCost()
    {
        return 200L * (Stats.MoreStellarEnergyLevel + 1);
    }

    public bool TryUpgradeMoreStellarEnergy(out long cost)
    {
        cost = GetMoreStellarEnergyUpgradeCost();

        if (Stats.MoreStellarEnergyLevel >= Stats.MaxMoreStellarEnergyLevel)
            return false;

        if (!Stats.TrySpendEnergy(cost))
            return false;

        return Stats.TryUpgradeMoreStellarEnergy();
    }

    public long GetFasterLevelUpUpgradeCost()
    {
        return 1L + Stats.FasterLevelUpLevel;
    }

    public bool TryUpgradeFasterLevelUp(out long cost)
    {
        cost = GetFasterLevelUpUpgradeCost();

        if (Stats.FasterLevelUpLevel >= Stats.MaxFasterLevelUpLevel)
            return false;

        if (!Stats.TrySpendStellarEnergy(cost))
            return false;

        return Stats.TryUpgradeFasterLevelUp();
    }

    public long GetStellarMoreStellarEnergyUpgradeCost()
    {
        return 1L + Stats.StellarMoreStellarEnergyLevel;
    }

    public bool TryUpgradeStellarMoreStellarEnergy(out long cost)
    {
        cost = GetStellarMoreStellarEnergyUpgradeCost();

        if (Stats.StellarMoreStellarEnergyLevel >= Stats.MaxStellarMoreStellarEnergyLevel)
            return false;

        if (!Stats.TrySpendStellarEnergy(cost))
            return false;

        return Stats.TryUpgradeStellarMoreStellarEnergy();
    }

    public double GetSpawnIntervalSeconds()
    {
        var interval = BaseSpawnIntervalSeconds - (Stats.FasterCoresLevel * FasterCoresReductionPerLevel);
        return Math.Max(MinSpawnIntervalSeconds, interval);
    }

    public int GetBaseSpawnTier()
    {
        return 1 + Stats.BetterCoresLevel;
    }

    public double GetExtraTierChance()
    {
        return Stats.BetterCores2Level / 100d;
    }

    public double GetOfflineProductionMultiplier()
    {
        return 1d + (Stats.OfflineProductionLevel / 100d);
    }

    public double GetStellarEnergyMergeChance()
    {
        return BaseStellarEnergyMergeChance + (Stats.MoreStellarEnergyLevel * MoreStellarEnergyChancePerLevel);
    }

    public double GetStellarEnergySpawnChance()
    {
        return Stats.StellarMoreStellarEnergyLevel * StellarSpawnStellarEnergyChancePerLevel;
    }

    public double GetMergeXpMultiplier()
    {
        return 1d + (Stats.FasterLevelUpLevel * FasterLevelUpXpBonusPerLevel);
    }

    private int GetNextSpawnTier()
    {
        var tier = GetBaseSpawnTier();

        if (Random.Shared.NextDouble() < GetExtraTierChance())
            tier++;

        return tier;
    }

    private long RollStellarEnergyRewardOnMerge()
    {
        return Random.Shared.NextDouble() < GetStellarEnergyMergeChance() ? 1L : 0L;
    }

    private long RollStellarEnergyRewardOnSpawn()
    {
        return Random.Shared.NextDouble() < GetStellarEnergySpawnChance() ? 1L : 0L;
    }

    // -------------------------
    // CORE SYSTEM
    // -------------------------

    public bool TrySpawnCore(out long stellarEnergyGained)
    {
        stellarEnergyGained = 0;

        var nextTier = GetNextSpawnTier();
        var success = Board.TrySpawnCore(nextTier, out _);
        if (!success)
            return false;

        stellarEnergyGained = RollStellarEnergyRewardOnSpawn();
        if (stellarEnergyGained > 0)
        {
            Stats.AddStellarEnergy(stellarEnergyGained);
        }

        return true;
    }

    public bool TryMergeCores(Guid firstCoreId, Guid secondCoreId, out long xpGained, out long stellarEnergyGained)
    {
        xpGained = 0;
        stellarEnergyGained = 0;

        if (!Board.TryMerge(firstCoreId, secondCoreId, out var merged) || merged is null)
            return false;

        var baseXp = merged.Tier;
        xpGained = (long)Math.Max(1, Math.Floor(baseXp * GetMergeXpMultiplier()));
        stellarEnergyGained = RollStellarEnergyRewardOnMerge();

        Stats.RegisterMerge(xpGained, stellarEnergyGained, 0);
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
            var multiplier = GetOfflineProductionMultiplier();
            var energyEarned = (long)Math.Floor(cappedSeconds * productionPerSecond * multiplier);

            if (energyEarned > 0)
            {
                Stats.AddEnergy(energyEarned);
                _pendingOfflineEnergy = energyEarned;
                _pendingOfflineSeconds = cappedSeconds;
            }
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