using System;

namespace Webgame.Contracts.Upgrades;

public sealed record UpgradeCatalogEntry(
    string Key,
    string Name,
    int CurrentLevel,
    long NextCost,
    string EffectDescription,
    bool CanAfford,
    string CurrencyType
);