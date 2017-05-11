using Microsoft.VisualStudio.TestTools.UnitTesting;
using FortressCraft.ModFoundation.Block;

namespace ModFoundationTest
{
[TestClass]
public class BoxTest
{
    [TestMethod]
    public void ExtendOnce()
    {
        var box = new Box(new Position(10, 10, 10), new Position(12, 12, 12));
        var plusBox = new Box(new Position(10, 10, 10), new Position(13, 12, 12));
        var minusBox = new Box(new Position(10, 10, 9), new Position(12, 12, 12));
        Assert.AreEqual(plusBox, box.Extended(Direction.PlusX()));
        Assert.AreEqual(minusBox, box.Extended(Direction.MinusZ()));
    }

    [TestMethod]
    public void RelationEquals()
    {
        var box = new Box(new Position(2, 3, 41), new Position(123, 123 , 312));
        var another = new Box(new Position(2, 3, 41), new Position(123, 123 , 312));
        Assert.AreEqual(BoxRelation.Equals, box.Relation(another));
        Assert.AreEqual(BoxRelation.Equals, another.Relation(box));
    }

    [TestMethod]
    public void RelationContain()
    {
        var small = new Box(new Position(10, 10, 10), new Position(12, 12, 12));
        var big = new Box(new Position(10, 10, 9), new Position(15, 12, 15));
        Assert.AreEqual(BoxRelation.Contains, big.Relation(small));
        Assert.AreEqual(BoxRelation.Contained, small.Relation(big));
    }

    [TestMethod]
    public void IntersectsByCorner()
    {
        var one = new Box(new Position(10, 110, 110), new Position(20, 120, 120));
        var two = new Box(new Position(15, 115, 115), new Position(25, 125, 125));
        Assert.AreEqual(BoxRelation.Intersects, one.Relation(two));
        Assert.AreEqual(BoxRelation.Intersects, two.Relation(one));
    }

    [TestMethod]
    public void ItersectsFace()
    {
        var one = new Box(new Position(20, 20, 20), new Position(40, 40, 40));
        var two = new Box(new Position(25, 25, 15), new Position(30, 30, 45));
        Assert.AreEqual(BoxRelation.Intersects, one.Relation(two));
        Assert.AreEqual(BoxRelation.Intersects, two.Relation(one));
    }
}
}