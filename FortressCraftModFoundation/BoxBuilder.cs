using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FortressCraft.ModFoundation.Block;

namespace FortressCraft.ModFoundation.Multiblock
{
public class BoxBuilder : Builder
{
    public BoxBuilder(Position size, Materials materials)
    {
        this.Size = size;
        this.Materials = materials;
    }

    public bool BuildIfPossible(ModCheckForCompletedMachineParameters parameters)
    {
        var box = this.FilledBox(new BlockSurveyor(parameters.Frustrum), new Position(parameters));
        if (box != null)
        {Debug.Log("possible");
            var controlPosition = box.Value.Center();
            BuilderUtil.Build(
                parameters.Frustrum,
                new GridBox(box.Value).Blocks(),
                Materials,
                box.Value.Center()
            );
            return true;
        }Debug.Log("not possible");
        return false;
    }

    Box? FilledBox(BlockSurveyor surveyor, Position finishingBlock)
    {
        return new FilledBoxChecker(surveyor, finishingBlock, this.Size, this.Materials.Placements[0]).MachineSpace();
    }

    readonly Materials Materials;
    readonly Position Size;
}
}
