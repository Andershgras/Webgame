using System;
using System.Collections.Generic;

namespace Webgame.Contracts.Players;

public sealed record PlayerResponse(
    Guid Id,
    string Name,

    int Level,
    long Experience,
    long ExperienceToNextLevel,

    long Energy,
    long StellarEnergy,
    long StormEnergy,

    int FasterCoresLevel,
    int BetterCoresLevel,
    int BetterCores2Level,
    int OfflineProductionLevel,
    double SpawnIntervalSeconds,

    int BoardSlotCount,
    IReadOnlyList<CoreDto> Cores,

    int OfflineCapSeconds,
    long OfflineEnergyEarned,
    int OfflineSecondsApplied,

    long TotalEnergyEarned,
    long TotalEnergySpent,
    long TotalMerges
);

public sealed record CoreDto(
    Guid Id,
    int Tier,
    int SlotIndex,
    long ProductionPerSecond
);  