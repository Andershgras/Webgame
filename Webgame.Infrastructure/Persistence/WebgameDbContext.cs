using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Webgame.Domain.Players;

namespace Webgame.Infrastructure.Persistence;

public sealed class WebgameDbContext : DbContext
{
    public WebgameDbContext(DbContextOptions<WebgameDbContext> options)
        : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WebgameDbContext).Assembly);
    }
}

