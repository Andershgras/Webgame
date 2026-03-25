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

        builder.Property(p => p.LastActiveUtc)
            .IsRequired();

        builder.OwnsOne(p => p.Stats, stats =>
        {
            stats.Property(s => s.Level)
                .HasColumnName("Level")
                .IsRequired();

            stats.Property(s => s.Experience)
                .HasColumnName("Experience")
                .IsRequired();

            stats.Property(s => s.ExperienceToNextLevel)
                .HasColumnName("ExperienceToNextLevel")
                .IsRequired();

            stats.Property(s => s.Energy)
                .HasColumnName("Energy")
                .IsRequired();

            stats.Property(s => s.StellarEnergy)
                .HasColumnName("StellarEnergy")
                .IsRequired();

            stats.Property(s => s.StormEnergy)
                .HasColumnName("StormEnergy")
                .IsRequired();

            stats.Property(s => s.OfflineCapLevel)
                .HasColumnName("OfflineCapLevel")
                .IsRequired();

            stats.Property(s => s.TotalEnergyEarned)
                .HasColumnName("TotalEnergyEarned")
                .IsRequired();

            stats.Property(s => s.TotalEnergySpent)
                .HasColumnName("TotalEnergySpent")
                .IsRequired();

            stats.Property(s => s.TotalMerges)
                .HasColumnName("TotalMerges")
                .IsRequired();

            stats.Ignore(s => s.OfflineCapSeconds);
        });

        builder.OwnsOne(p => p.Board, board =>
        {
            board.Property(b => b.SlotCount)
                .HasColumnName("BoardSlotCount")
                .IsRequired();

            board.OwnsMany(b => b.Cores, cores =>
            {
                cores.ToTable("PlayerCores");

                cores.WithOwner().HasForeignKey("PlayerId");

                cores.HasKey("Id");

                cores.Property(c => c.Id)
                    .ValueGeneratedNever();

                cores.Property(c => c.Tier)
                    .IsRequired();

                cores.Property(c => c.SlotIndex)
                    .IsRequired();
            });

            board.Navigation(b => b.Cores)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        builder.Ignore("_pendingOfflineEnergy");
        builder.Ignore("_pendingOfflineSeconds");

        builder.Navigation(p => p.Stats).IsRequired();
        builder.Navigation(p => p.Board).IsRequired();
    }
}