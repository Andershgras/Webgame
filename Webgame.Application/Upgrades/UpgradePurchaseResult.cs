using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webgame.Domain.Players;

namespace Webgame.Application.Upgrades;
public sealed record UpgradePurchaseResult(
    string Key,
    long Cost,
    int NewLevel,
    Player Player
);

