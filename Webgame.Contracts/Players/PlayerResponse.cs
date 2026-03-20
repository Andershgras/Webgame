using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webgame.Contracts.Players;

public sealed record PlayerResponse(
    Guid Id,
    string Name,
    int Level,
    long Experience,
    long ExperienceToNextLevel,
    long Coins,
    int ClickPower,
    int ClickPowerLevel,
    int AutoClickerLevel,

    int ClicksPerSecond,
    int OfflineCapSeconds,
    long OfflineCoinsEarned,
    int OfflineSecondsApplied,

    long TotalClicks,
    long TotalCoinsEarned,
    long TotalCoinsSpent
);
