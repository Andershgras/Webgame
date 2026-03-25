using System;

namespace Webgame.Domain.Players;

public sealed class CoreInstance
{
    private CoreInstance() { } // EF Core

    public Guid Id { get; private set; }
    public int Tier { get; private set; }
    public int SlotIndex { get; private set; }

    public CoreInstance(Guid id, int tier, int slotIndex)
    {
        if (tier < 1)
            throw new ArgumentOutOfRangeException(nameof(tier));

        if (slotIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(slotIndex));

        Id = id;
        Tier = tier;
        SlotIndex = slotIndex;
    }

    public static CoreInstance Create(int tier, int slotIndex)
        => new(Guid.NewGuid(), tier, slotIndex);

    public static CoreInstance CreateTier1(int slotIndex)
        => new(Guid.NewGuid(), 1, slotIndex);

    public long GetProductionPerSecond()
    {
        return (long)Math.Pow(3, Tier - 1);
    }

    public void MoveToSlot(int slotIndex)
    {
        if (slotIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(slotIndex));

        SlotIndex = slotIndex;
    }

    public CoreInstance CreateMergedInto(int targetSlotIndex)
    {
        return new CoreInstance(Guid.NewGuid(), Tier + 1, targetSlotIndex);
    }
}