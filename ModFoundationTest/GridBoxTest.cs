using FortressCraft.ModFoundation.Block;
using FortressCraft.ModFoundation.Multiblock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ModFoundationTest
{
[TestClass]
public class GridBoxTest
{
    [TestMethod]
    public void CountSides()
    {
        var mult = new GridBox(new Box(new Position(1, 1, 2), new Position(3, 2, 2)));
        var count = mult.Sides().Count();
        Assert.AreEqual(count, 22);
    }

    [TestMethod]
    public void FindSide()
    {
        var mult = new GridBox(new Box(new Position(-5, 1, -2), new Position(3, 4, 3)));
        var side = new Position(-6, 2, 0);
        var exist = mult.Sides().First((s) => s == side);
    }

    [TestMethod]
    public void OneCubeSides()
    {
        var mult = new GridBox(new Box(new Position(8, -4, 0), new Position(8, -4, 0)));
        var sideCoords = new [] {
            new Position(8, -5, 0),
            new Position(8, -3, 0),
            new Position(9, -4, 0),
            new Position(7, -4, 0),
            new Position(8, -4, -1),
            new Position(8, -4, 1),
        };

        foreach (var side in mult.Sides())
        {
            var exist = sideCoords.First( (c) => c == side);
        }
    }

    [TestMethod]
    public void OneCubeBlock()
    {
        var block = new Position(4, 4, 4);
        var mult = new GridBox(new Box(block, block));
        var exist = mult.Blocks().First((b) => b == block);
        Assert.AreEqual(1, mult.Blocks().Count());
    }

    [TestMethod]
    public void LargerBlock()
    {
        var mult = new GridBox(new Box(new Position(-1, 4, 2), new Position(1, 4, 2)));
        var cubeCoords = new [] {
            new Position(-1, 4, 2),
            new Position(0, 4, 2),
            new Position(1, 4, 2)
        };

        foreach (var block in mult.Blocks())
        {
            var exist = cubeCoords.First((b) => b == block);
        }
    }
}
}
