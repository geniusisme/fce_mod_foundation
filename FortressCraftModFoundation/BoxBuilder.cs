using UnityEngine;
using FortressCraft.ModFoundation.Block;

namespace FortressCraft.ModFoundation.Multiblock
{
public class BoxBuilder : Builder
{
    public BoxBuilder(Position size, Materials materials, Orientation orientation)
    {
        this.Size = size;
        this.Materials = materials;
        this.Orientation = orientation;
    }

    public bool BuildIfPossible(ModCheckForCompletedMachineParameters parameters)
    {
        var box = this.FilledBox(new BlockSurveyor(parameters.Frustrum), new Position(parameters));
        if (box != null)
        {
            BuilderUtil.Build(
                parameters.Frustrum,
                new GridBox(box.Value).Blocks(),
                this.Materials,
                box.Value.Center(),
                this.Orientation
            );
            return true;
        }
        return false;
    }

    Box? FilledBox(BlockSurveyor surveyor, Position finishingBlock)
    {
        return new FilledBoxChecker(surveyor, finishingBlock, this.Size, this.Materials.Placements[0]).MachineSpace();
    }

    readonly Materials Materials;
    readonly Position Size;
    readonly Orientation Orientation;
}
}
