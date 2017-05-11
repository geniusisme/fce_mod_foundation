using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine;
using FortressCraft.ModFoundation;
using FortressCraft.ModFoundation.Block;

using Material = FortressCraft.ModFoundation.Block.Material;

namespace ModFoundationTest
{
[TestClass]
public class SurveyorTest
{
    SegmentProviderStub Provider = new SegmentProviderStub();
    BlockSurveyor Surveyor;

    public SurveyorTest()
    {
        this.Surveyor = new BlockSurveyor(this.Provider);
    }

    [TestMethod]
    public void FindMaterial()
    {
        var position = new Position(3, 3, 4);
        var material = new Material(42, 24);
        this.Provider.SetMaterial(material, position);
        var block = this.Surveyor.Look().At(position).Block();
        Assert.AreEqual(material, block);
    }

    [TestMethod]
    public void FindMaterialByType()
    {
        var position1 = new Position(3, 3, 4);
        var material1 = new Material(42, 1);
        this.Provider.SetMaterial(material1, position1);
        var material2 = new Material(42, 2);
        var position2 = new Position(3, 3, 2341);
        this.Provider.SetMaterial(material2, position2);
        var block = this.Surveyor.Look().At(position1).WhereBlockIs(material1.Type).Block();
        Assert.AreEqual(material1.Type, block.Value.Type);
        // this is not a type, we are looking for the same type
        block = this.Surveyor.Look().At(position2).WhereBlockIs(material1.Type).Block();
        Assert.AreEqual(material2.Type, block.Value.Type);
    }

    [TestMethod]
    public void FailToFindMaterial()
    {
        var position = new Position(3, 3, 4);
        var materialToFind = new Material(42, 1);
        var materialExisting = new Material(42, 2);
        this.Provider.SetMaterial(materialExisting, position);
        var block = this.Surveyor.Look().At(position).WhereBlockIs(materialToFind).Block();
        Assert.AreEqual(null, block);
    }

    [TestMethod]
    public void FindEntity()
    {
        var position = new Position(432412, 432432, 145423);
        var material = new Material(4, 123);
        this.Provider.AddEntity(new SegmentEntityStub(position), material);
        var entity = this.Surveyor.Look().At(position).For<SegmentEntityStub>();
        Assert.IsTrue(entity != null);
    }

    [TestMethod]
    public void FailToFindEntityOfMaterial()
    {
        var position = new Position(432412, 432432, 145423);
        var materialToFind = new Material(42, 1);
        var materialExisting = new Material(42, 2);
        this.Provider.AddEntity(new SegmentEntityStub(position), materialExisting);
        var entity = this.Surveyor.Look().At(position).WhereBlockIs(materialToFind).For<SegmentEntityStub>();
        Assert.IsTrue(entity == null);
    }
}

class SegmentEntityStub : SegmentEntity
{
    public SegmentEntityStub(Position position):
        base(eSegmentEntity.InjectionEntity, position.X, position.Y, position.Z, Vector3.zero, null)
    {
    }
}
}
