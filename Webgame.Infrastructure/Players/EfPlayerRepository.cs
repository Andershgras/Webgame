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
        await _db.Players.AddAsync(player, ct);
    }

    public Task<Player?> GetByIdAsync(PlayerId id, CancellationToken ct)
    {
        // Tracked entity – ændringer bliver opdaget af EF (men vi kalder Update eksplicit for pædagogik nu)
        return _db.Players.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public void Update(Player player)
    {
        _db.Players.Update(player);
    }

    public void Remove(Player player)
    {
        _db.Players.Remove(player);
    }
}
