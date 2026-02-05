using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Webgame.Application.Players;
using Webgame.Domain.Players;
using Webgame.Infrastructure.Persistence;

namespace Webgame.Infrastructure.Players;

public sealed class EfPlayerRepository : IPlayerRepository
{
    private readonly WebgameDbContext _db;

    public EfPlayerRepository(WebgameDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Player player, CancellationToken ct)
    {
        // Hvis den ikke er tracked endnu, attach/add.
        var exists = await _db.Players.AnyAsync(p => p.Id == player.Id, ct);

        if (!exists)
            _db.Players.Add(player);

        await _db.SaveChangesAsync(ct);
    }

    public Task<Player?> GetByIdAsync(PlayerId id, CancellationToken ct)
    {
        return _db.Players.FirstOrDefaultAsync(p => p.Id == id, ct);
    }
}

