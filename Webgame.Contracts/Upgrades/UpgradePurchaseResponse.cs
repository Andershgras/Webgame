using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webgame.Contracts.Players;

namespace Webgame.Contracts.Upgrades;
public sealed record UpgradePurchaseResponse(
    string Key,
    long Cost,
    int NewLevel,
    PlayerResponse Player
);
