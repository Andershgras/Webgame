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

        builder.HasIndex(p => p.Name).IsUnique();

        builder.Property(p => p.Name)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.PasswordHash)
            .IsRequired();

        builder.OwnsOne(p => p.Stats, stats =>
        {
            stats.Property(s => s.Currency)
                .HasColumnName("Currency")
                .HasDefaultValue(0)
                .IsRequired();

            stats.Property(s => s.HasUnlockedFirstGame)
                .HasColumnName("HasUnlockedFirstGame")
                .HasDefaultValue(false)
                .IsRequired();
        });

        builder.OwnsMany(p => p.Games, game =>
        {
            game.ToTable("PlayerGames");

            game.WithOwner().HasForeignKey("PlayerId");

            game.HasKey(g => g.Id);

            game.Property(g => g.Id)
                .ValueGeneratedNever();

            game.Property(g => g.Name)
                .HasMaxLength(100)
                .IsRequired();

            game.Property(g => g.Players)
                .IsRequired();

            game.Property(g => g.Revenue)
                .IsRequired();
        });

        builder.Navigation(p => p.Games).AutoInclude();
    }
}
