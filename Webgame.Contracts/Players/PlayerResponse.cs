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

    // NEW: Board
    int BoardSlotCount,
    IReadOnlyList<CoreDto> Cores,

    // Temporary legacy fields
    int ClickPower,
    int ClickPowerLevel,
    int AutoClickerLevel,
    int ClicksPerSecond,

    int OfflineCapSeconds,
    long OfflineEnergyEarned,
    int OfflineSecondsApplied,

    long TotalClicks,
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