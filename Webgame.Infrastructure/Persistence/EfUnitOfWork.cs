using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webgame.Application.Persistence;

namespace Webgame.Infrastructure.Persistence;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly WebgameDbContext _db;

    public EfUnitOfWork(WebgameDbContext db)
    {
        _db = db;
    }

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}

