using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webgame.Contracts.Upgrades;

public sealed record UpgradeCatalogEntry(
    string Key,
    string Name,
    int CurrentLevel,
    long NextCost,
    string EffectDescription,
    bool CanAfford
);
