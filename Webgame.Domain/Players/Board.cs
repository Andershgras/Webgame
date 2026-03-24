using System;
using System.Collections.Generic;
using System.Linq;

namespace Webgame.Domain.Players;

public sealed class Board
{
    private readonly List<CoreInstance> _cores = new();

    private Board() { } // EF Core

    public int SlotCount { get; private set; } = 12;

    public IReadOnlyCollection<CoreInstance> Cores => _cores.AsReadOnly();

    public Board(int slotCount = 12)
    {
        if (slotCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(slotCount));

        SlotCount = slotCount;
    }

    public bool HasFreeSlot()
    {
        return _cores.Count < SlotCount;
    }

    public int? GetFirstFreeSlot()
    {
        for (int i = 0; i < SlotCount; i++)
        {
            if (_cores.All(c => c.SlotIndex != i))
                return i;
        }

        return null;
    }

    public bool TrySpawnTier1Core(out CoreInstance? spawnedCore)
    {
        spawnedCore = null;

        var freeSlot = GetFirstFreeSlot();
        if (freeSlot is null)
            return false;

        spawnedCore = CoreInstance.CreateTier1(freeSlot.Value);
        _cores.Add(spawnedCore);
        return true;
    }

    public bool TryMerge(Guid firstCoreId, Guid secondCoreId, out CoreInstance? mergedCore)
    {
        mergedCore = null;

        if (firstCoreId == secondCoreId)
            return false;

        var first = _cores.FirstOrDefault(c => c.Id == firstCoreId);
        var second = _cores.FirstOrDefault(c => c.Id == secondCoreId);

        if (first is null || second is null)
            return false;

        if (first.Tier != second.Tier)
            return false;

        var targetSlot = Math.Min(first.SlotIndex, second.SlotIndex);

        _cores.Remove(first);
        _cores.Remove(second);

        mergedCore = first.CreateMergedInto(targetSlot);
        _cores.Add(mergedCore);

        return true;
    }

    public long GetProductionPerSecond()
    {
        return _cores.Sum(c => c.GetProductionPerSecond());
    }
}