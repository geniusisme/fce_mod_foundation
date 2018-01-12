using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FortressCraft.ModFoundation.Block;

namespace FortressCraft.ModFoundation
{
using EntityCreator = System.Func<ModCreateSegmentEntityParameters, SegmentEntity>;

/// base class for modding, greatly simplifies your work, with combination with the rest of the assembly
public class Mod : FortressCraftMod
{
    /// override this method and call RegisterBlock() and Register(Box)Multiblock from it
    public virtual void RegisterEntities() {}

    public virtual String Name()
    {
        return "Do not forget to override your mod name";
    }

    public sealed override ModRegistrationData Register()
    {
        RegisterEntities();
        Debug.Log("Registered Mod: " + this.Name());
        return this.RegistrationData;
    }

    /// register your block entity if you want to see it in the game
    /// provide your key from TerrainData.xml and creator for your entity, eg.
    /// RegisterBlock("JohnDoe.AwsomeBlock", (p) => new AwsomeEntity(p))
    /// If block have different values, keys for all values should be registered.
    protected Block.Material RegisterBlock(string key, EntityCreator creator)
    {
        Debug.Log("registering block: " + key);
        var block = new Block.Material(key);
        this.BlockRegistrations[block] = creator;
        this.RegistrationData.RegisterEntityHandler(key);
        Debug.Log("block: " + key + " registered successfully as: " + block.ToString());
        return block;
    }

    /// register your multiblock machine if you want to see it in the game
    /// use this method only if your machine is built from box, filled with the same (type and value) blocks
    /// key is prefix, shared by all your TerrainData.xml entries, corresponding to the machine
    /// size denotes the box containing machine
    /// creators create your machine, as they do in single block registration, eg.
    /// (p) => new BigAwsomeEntity(p),
    /// (p) => new Filler<BigAwsomeEntity>(p)
    protected Multiblock.Materials RegisterBoxMultiblock(
        string key,
        Position size,
        EntityCreator controlCreator,
        EntityCreator fillerCreator
    )
    {
        if (size.Any((c) => c < 1))
        {
            Debug.Log("multiblock " + key + " doesn't have positive volume, fix its size");
        }
        var materials = this.RegisterMultiblockNoBuilder(key, new List<string> {"Placement"}, controlCreator, fillerCreator);
        this.RegisterMultiblockBuilder(materials, new Multiblock.BoxBuilder(size, materials, Orientation.Identity()));
        return materials;
    }

    /// register your multiblock machine if you want to see it in the game
    /// this method can be used to create complex entities
    /// key is prefix for your enties for Control and Filler blocks in TerrainData.xml file
    /// placementSuffixes is a list of all the suffixes such as TerrainData.xml has entry key + placementSuffix[i]
    /// that way you can build machine from different blocks
    /// creators are the same as for RegisterBoxMultiblock
    /// builder controls how and from what machine is being built
    /// it is recommended to save result of this method in public static to access it from machine constructor
    protected Multiblock.Materials RegisterMultiblock(
        string key,
        List<string> placementSuffixes,
        EntityCreator controlCreator,
        EntityCreator fillerCreator,
        Multiblock.Builder builder
    )
    {
        var materials =  this.RegisterMultiblockNoBuilder(key, placementSuffixes, controlCreator, fillerCreator);
        this.RegisterMultiblockBuilder(materials, builder);
        return materials;
    }

    Multiblock.Materials RegisterMultiblockNoBuilder(
        string key,
        List<string> placementSuffixes,
        EntityCreator controlCreator,
        EntityCreator fillerCreator
    )
    {
        var controlBlock = this.RegisterBlock(key + "Control", controlCreator);
        var fillerBlock = this.RegisterBlock(key + "Filler", fillerCreator);
        var placements = placementSuffixes.Select((suffix) => this.RegisterBlock(key + suffix, (p) => null)).ToList();
        TerrainData.mEntries[controlBlock.Type].PickReplacement = key + placementSuffixes[0];
        TerrainData.mEntries[fillerBlock.Type].PickReplacement = key + placementSuffixes[0];
        return new Multiblock.Materials
        {
            Placements = placements,
            Control = controlBlock,
            Filler = fillerBlock,
        };
    }

    void RegisterMultiblockBuilder(Multiblock.Materials materials, Multiblock.Builder builder)
    {
        foreach (var placement in materials.Placements)
        {
            this.MultiblockBuilders[placement] = builder;
        }
    }

    public sealed override void CreateSegmentEntity(ModCreateSegmentEntityParameters parameters, ModCreateSegmentEntityResults results)
    {
        var material = new Block.Material(parameters.Cube, parameters.Value);
        parameters.ObjectType = SpawnableObjectEnum.Num;
        results.Entity = this.BlockRegistrations[material](parameters);
    }

    public sealed override void CheckForCompletedMachine(ModCheckForCompletedMachineParameters parameters)
    {
        const int MachinePlacement = 600;
        var material = new Block.Material(MachinePlacement, parameters.CubeValue);
        this.MultiblockBuilders[material].BuildIfPossible(parameters);
    }

    Dictionary<Block.Material, EntityCreator> BlockRegistrations = new Dictionary<Block.Material, EntityCreator>();
    Dictionary<Block.Material, Multiblock.Builder> MultiblockBuilders = new Dictionary<Block.Material, Multiblock.Builder>();
    ModRegistrationData RegistrationData = new ModRegistrationData();
}
}