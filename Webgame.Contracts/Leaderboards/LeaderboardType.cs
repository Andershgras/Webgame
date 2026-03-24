namespace Webgame.Contracts.Leaderboards;

public enum LeaderboardType
{
    TotalClicks = 0,

    // Legacy naming kept for compatibility (maps to Energy internally)
    TotalCoinsEarned = 1,
    TotalCoinsSpent = 2,

    // New preferred names
    TotalEnergyEarned = 3,
    TotalEnergySpent = 4,
    TotalMerges = 5,
    Level = 6
}