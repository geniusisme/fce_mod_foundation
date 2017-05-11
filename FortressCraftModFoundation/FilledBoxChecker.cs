using System;
using System.Collections.Generic;
using System.Linq;
using FortressCraft.ModFoundation.Block;

using Material = FortressCraft.ModFoundation.Block.Material;

namespace FortressCraft.ModFoundation.Multiblock
{
public class FilledBoxChecker
{
    public FilledBoxChecker(
        BlockSurveyor surveyor,
        Position finishingBlock,
        Position size,
        Material filler)
    {
        this.Surveyor = surveyor;
        this.FinishingBlock = finishingBlock;
        this.Size = size;
        this.Filler = filler;
    }

    public Box? MachineSpace()
    {
        if (this.Surveyor.Look().At(this.FinishingBlock).Block() != this.Filler)
        {
            return null;
        }
        if (this.Size == new Position(1, 1, 1))
        {
            return new Box(this.FinishingBlock, this.FinishingBlock);
        }
        this.SetupInitialSearchArea();
        if (this.SearchArea.Count == 0)
        {
            return null;
        }
        return DepthSearch();
    }

    void SetupInitialSearchArea()
    {
        var box = new Box(this.FinishingBlock, this.FinishingBlock);
        var maxExtents = this.Size.Concat(this.Size).Select((coord) => (int)(coord - 1)).ToArray();
        foreach (var direction in Direction.All())
        {
            var toSurvey = this.FinishingBlock;
            for (var extent = 1; extent <= maxExtents[direction.Index]; ++extent)
            {
                toSurvey = toSurvey + direction.Shift;
                if (Surveyor.Look().At(toSurvey).Block() == this.Filler)
                {
                    box = box.Extended(direction);
                }
                else
                {
                    break;
                }
            }
        }
        this.TryAddBoxToSearchArea(box);
        this.Box = box;
    }

    void TryAddBoxToSearchArea(Box box)
    {
        if (box.Outfits(this.Size))
        {
            this.SearchArea.Add(box);
        }
    }

    Box? DepthSearch()
    {
        Stack<Box> open = new Stack<Box>();
        HashSet<Box> generated = new HashSet<Box>();
        open.Push(new Box(this.FinishingBlock, this.FinishingBlock));
        while (open.Count > 0)
        {
            var next = open.Pop();
            foreach (var direction in Direction.All())
            {
                var extended = next.Extended(direction);
                if (generated.Contains(extended))
                {
                    continue;
                }
                generated.Add(extended);
                if (!extended.FitsIn(this.Size) || !this.BelongsToSearchArea(extended))
                {
                    continue;
                }
                var impediment = new GridBox(next).Side(direction)
                    .Find((b) => this.Surveyor.Look().At(b).Block() != this.Filler);

                if (impediment.HasValue)
                {
                    this.AddImpediment(impediment.Value);
                    if (this.SearchArea.Count == 0)
                    {
                        return null;
                    }
                }
                else
                {
                    if (extended.Size() == this.Size)
                    {
                        return extended;
                    }
                    open.Push(extended);
                }
            }
        }
        return null;
    }

    void AddImpediment(Position impediment)
    {
        var center = this.FinishingBlock;
        var coords = Enumerable.Range(0, 3);
        var commonCorner = coords.Select((i) => this.BoxCorner(impediment[i] > center[i])[i]).ToPosition();

        this.SearchArea = coords
            .Where(i => center[i] - impediment[i] != 0)
            .Select((iBox) => coords
                .Select((i) => (i == iBox)?
                    impediment[i] + Math.Sign(center[i] - impediment[i]):
                    this.BoxCorner(impediment[i] <= center[i])[i]))
            .Select((e) => new Position(e))
            .Select((c) => new Box(c, commonCorner))
            .SelectMany((b) => this.SearchArea.SelectValues((a) => a.Intersect(b)))
            .Where((b) => b.Outfits(this.Size))
            .ToHashSet();
    }

    Position BoxCorner(bool min)
    {
        return min? this.Box.Min: this.Box.Max;
    }

    bool BelongsToSearchArea(Box box)
    {
        return this.SearchArea.Any((s) => s.Outfits(box));
    }

    HashSet<Box> SearchArea = new HashSet<Box>();
    BlockSurveyor Surveyor;
    Position FinishingBlock;
    Position Size;
    Material Filler;
    Box Box;
}
}