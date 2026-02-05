using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Webgame.Domain.Players;

namespace Webgame.Infrastructure.Persistence.Configurations;

public sealed class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("Players");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => new PlayerId(value)
            );

        builder.Property(p => p.Name)
            .HasMaxLength(20)
            .IsRequired();

        builder.OwnsOne(p => p.Stats, stats =>
        {
            stats.Property(s => s.Level).HasColumnName("Level").IsRequired();
            stats.Property(s => s.Coins).HasColumnName("Coins").IsRequired();
            stats.Property(s => s.ClickPower).HasColumnName("ClickPower").IsRequired();
        });

        builder.Navigation(p => p.Stats).IsRequired();
    }
}
