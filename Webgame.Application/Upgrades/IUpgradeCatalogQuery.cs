using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webgame.Application.Common;
using Webgame.Contracts.Upgrades;

namespace Webgame.Application.Upgrades;

public interface IUpgradeCatalogQuery
{
    Task<Result<IReadOnlyList<UpgradeCatalogEntry>>> GetForPlayerAsync(Guid playerId, CancellationToken ct);
}
