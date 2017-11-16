using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FortressCraft.ModFoundation.Block;

using static FortressCraft.ModFoundation.EnumeratorUtil;

namespace FortressCraft.ModFoundation.Multiblock
{
/// multiblock machines constist from several materials
public class Materials
{
    /// materials for blocks from which machine is assembled.
    /// for most machines it will be just one material
    public List<Block.Material> Placements;

    /// material for control block of assembled machine
    public Block.Material Control;

    /// material for all other blocks of assembled machine
    public Block.Material Filler;
}

public interface Builder
{
    /// this method should check if machine is ready for being assembled and then build it
    bool BuildIfPossible(ModCheckForCompletedMachineParameters parameters);
}

/// this is interface for block which is responsible for logic and graphics of multiblock machine
/// implement it like this:
/// class MyUberMachine : IControl<MyUberMachine>
/// {
///     public Connector<MyUberMachine> Connector { get; set; }
/// }
public interface IControl<Machine> where Machine : MachineEntity, IControl<Machine>
{
    Connector<Machine> Connector { get; set; }
}

/// this is base class for blocks of multiblock machine which are basically there to fill the space
/// if you are not implementing additional interfaces, like PowerConsumer, for your control block,
/// you do not even need to inherit, just use Filler<MyUberMachine>
/// if you do, however, implement additional interface, you should implement it for filler as well:
/// class MyUberFiller : Filler<MyUberMachine>, ISuper
/// {
///     void ISuper.Sup()
///     {
///         (this.Contol as ISuper)?.Sup();
///     }
/// }
/// It is important to account for this.Contol == null - this is state in which machine is not fully assembled
/// And generally just should do nothing.
public class Filler<Machine> : MachineEntity where Machine : MachineEntity, IControl<Machine>
{
    public Filler(ModCreateSegmentEntityParameters parameters)
        : base(parameters)
    {}

    public Machine Control { get; set; }

    public override string GetPopupText()
    {
        return this.Control == null? "" : this.Control.GetPopupText();
    }

    public override void SpawnGameObject()
    {
        return;
    }

    public override void OnDelete()
    {
        base.OnDelete();
        this.Control?.Connector?.Delink(this);
    }
}

/// this class is a core of all multiblock machinery.
/// it assmebles and disassembles multiblock machines using quite complex logic
public class Connector<Machine> where Machine : MachineEntity, IControl<Machine>
{
    /// create connector and set it as your IContol Connector propery in constructor
    /// for occupied space for box-like machines use GridBox.Blocks()
    public Connector(Machine machine, Materials materials, IEnumerable<Position> occupiedSpace)
    {
        this.Control = machine;
        var position = new Position(machine);
        this.Fillers = occupiedSpace.Where((b) => b != position).Index();
        this.Materials = materials;
        this.LinkState = LinkState.Linking;
        this.Linked = new bool[this.Fillers.Count()];
        machine.mbNeedsLowFrequencyUpdate = true;
    }

    /// call this method in LowFrequencyUpdate of your machine
    /// only perform actual update if it returns true
    public bool Operational()
    {
        switch (this.LinkState)
        {
            case LinkState.Linking: TryToLink(); return true;
            case LinkState.Linked: return true;
            case LinkState.Delinking: TryToDelink(); return false;
            case LinkState.Delinked: return false;
        }
        return false;
    }

    /// call this in OnDelete method of your machine
    public void OnDelete()
    {
        // if some segments are not in our frustrum yet, their content will never delink and be replaced by placement block
        // i can't see what we can do about that. probably some thread which will cleanup things
        if (this.LinkState != LinkState.Delinked)
        {
            this.StartDelinking();
            this.TryToDelink(delinkSelf: false);
        }
        this.DropItem(new Position(this.Control));
    }

    internal void Delink(Filler<Machine> filler)
    {
        this.StartDelinking();
        var delinkedPosition = new Position(filler);
        var delinkedIndex = this.Fillers.First((b) => b.Value == delinkedPosition).Index;
        // this is important to delink after start delinking, otherwise we will crash
        // it is also important to delink before TryToDelink, so this filler won't be replaced by placement block
        this.Delink(filler, delinkedIndex);
        this.DropItem(new Position(filler));
        this.TryToDelink();
    }

    void TryToLink()
    {
        var surveyor = new BlockSurveyor(this.Control);
        foreach (var block in this.Fillers.Where((f) => !this.Linked[f.Index]))
        {
            var material = surveyor.Look().At(block.Value).Block();
            // not loaded or actual filler block is not created yet
            if (material == null || IsPlacement(material.Value))
            {
                continue;
            }
            else if (material == this.Materials.Filler)
            {
                var filler = surveyor.Look().At(block.Value).For<Filler<Machine>>();
                if (filler == null)
                { // game put material in place, but not entity yet.
                    continue;
                }
                filler.Control = this.Control;
                filler.mSegment.RequestRegenerateGraphics();
                this.Linked[block.Index] = true;
            }
            // block was destroyed before link process completed, so it could not call delink in OnDestroy method
            // we should do it ourselves
            else
            {
                this.StartDelinking();
                this.TryToDelink();
                return;
            }
        }
        if (this.Fillers.All((f) => this.Linked[f.Index]))
        {
            this.Control.mSegment.RequestRegenerateGraphics();
            this.LinkState = LinkState.Linked;
        }
    }

    void TryToDelink(bool delinkSelf = true)
    {
        var surveyor = new BlockSurveyor(this.Control);
        foreach (var block in this.Fillers.Where((f) => !this.Delinked[f.Index]))
        {
            var material = surveyor.Look().At(block.Value).Block();
            if (material == null || IsPlacement(material.Value))
            {
                continue;
            }
            else if (material == this.Materials.Filler)
            {
                var filler = surveyor.Look().At(block.Value).For<Filler<Machine>>();
                this.Delink(filler, block.Index);
                this.Replace(filler);
            }
            // block is long gone, while we are still in delinking process
            else
            {
                this.Delinked[block.Index] = true;
            }
        }

        if (delinkSelf && this.Fillers.All((f) => this.Delinked[f.Index]))
        {
            this.LinkState = LinkState.Delinked;
            this.Replace(this.Control);
        }
    }

    void Delink(Filler<Machine> filler, int index)
    {
        this.Delinked[index] = true;
        filler.Control = null;
    }

    void StartDelinking()
    {
        if (this.LinkState != LinkState.Delinking)
        {
            this.LinkState = LinkState.Delinking;
            this.Delinked = new bool[this.Fillers.Count()];
            this.Linked = null;
        }
    }

    void DropItem(Position position)
    {
        if (WorldScript.mbIsServer)
        {
            ItemManager.DropNewCubeStack(this.Replacement.Type, this.Replacement.Value, 1, position.X, position.Y, position.Z, Vector3.zero);
        }
    }

    void Replace(SegmentEntity entity)
    {
        WorldScript.instance.BuildFromEntity(entity.mSegment, entity.mnX, entity.mnY, entity.mnZ, this.Replacement.Type, this.Replacement.Value);
    }

    Block.Material Replacement { get { return this.Materials.Placements[0]; } }

    bool IsPlacement(Block.Material material)
    {
        return this.Materials.Placements.Any((b) => b == material);
    }

    readonly Machine Control;
    readonly Materials Materials;
    readonly IEnumerable<IndexedValue<Position>> Fillers;
    LinkState LinkState;
    bool[] Linked;
    bool[] Delinked;
}

enum LinkState
{
    Linking,
    Linked,
    Delinking,
    Delinked,
}

public static class BuilderUtil
{
    /// this method build multiblock machine in place of placements blocks.
    /// fustrum - get that form ModCheckForCompletedMachineParameters
    /// blocks - all the positions machine should occupy
    /// contolPosition - position of the control block. It is important for reusing code of existing machines
    /// method does not check if given positions are filled with placement blocks, but they should at least be
    /// loaded into world
    /// use this method as foundation for implementing Builder interface (if BoxBuilder doesn't suit your needs)
    public static void Build(
        WorldFrustrum frustrum,
        IEnumerable<Position> blocks,
        Materials materials,
        Position controlPosition,
        Orientation orientation)
    {
        // Player builds with placement blocks, filler and control appear instead of them.
        WorldScript.mLocalPlayer.mResearch.GiveResearch(materials.Control.Type, materials.Control.Value);
        WorldScript.mLocalPlayer.mResearch.GiveResearch(materials.Filler.Type, materials.Filler.Value);

        foreach (var block in blocks)
        {
            if (block == controlPosition) {
                continue;
            }
            BuildPart(frustrum, block, materials.Filler, orientation.Flags());
        }
        BuildPart(frustrum, controlPosition, materials.Control, orientation.Flags());
        AudioSpeechManager.PlayStructureCompleteDelayed = true;
    }

    static void BuildPart(WorldFrustrum frustrum, Position block, Block.Material material, byte orientationFlags)
    {
        var segment = new FrustrumSegmentProvider(frustrum).GetSegment(block);
        if (segment == null)
        {
            Debug.Log("Attempt to build multiblock machine in non-ready segment. Always check filled boxes before build");
            return;
        }
        frustrum.BuildOrientation(segment, block.X, block.Y, block.Z, material.Type, material.Value, orientationFlags);
    }
}

}
