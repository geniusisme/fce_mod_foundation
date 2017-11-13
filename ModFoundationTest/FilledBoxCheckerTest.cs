using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FortressCraft.ModFoundation;
using FortressCraft.ModFoundation.Block;
using FortressCraft.ModFoundation.Multiblock;

using Material = FortressCraft.ModFoundation.Block.Material;

namespace ModFoundationTest
{
[TestClass]
public class FilledBoxCheckerTest
{
    SegmentProviderStub Provider = new SegmentProviderStub();
    BlockSurveyor Surveyor;
    Material Filler = new Material(312, 123);

    public FilledBoxCheckerTest()
    {
        this.Surveyor = new BlockSurveyor(this.Provider);
    }

    [TestMethod]
    public void FindSimpleBox()
    {
        var box = new Box(new Position(10, 10, 10), new Position(12, 12, 12));
        foreach (var block in new GridBox(box).Blocks())
        {
            this.Provider.SetMaterial(this.Filler, block);
        }

        var checker = new FilledBoxChecker(this.Surveyor, new Position(10, 11, 10), new Position(3, 3, 3), this.Filler);

        Assert.AreEqual(box, checker.MachineSpace().Value);
    }

    [TestMethod]
    public void FindSingleBlock()
    {
        var box = new Box(new Position(10, 10, 10), new Position(10, 10, 10));

        this.Provider.SetMaterial(this.Filler, box.Min);

        var checker = new FilledBoxChecker(this.Surveyor, new Position(10, 10, 10), new Position(1, 1, 1), this.Filler);

        Assert.AreEqual(box, checker.MachineSpace().Value);
    }

    [TestMethod]
    public void DoNotGetConfusedByManyPossibilities()
    {
        var box = new Box(new Position(10, 10, 10), new Position(12, 12, 12));
        foreach (var block in new GridBox(new Box(new Position(8, 8, 8), new Position(15, 15, 15))).Blocks())
        {
            this.Provider.SetMaterial(this.Filler, block);
        }

        var finishingBlock = new Position(10, 11, 10);
        var checker = new FilledBoxChecker(this.Surveyor, finishingBlock, new Position(3, 3, 3), this.Filler);
        var grid = new GridBox(checker.MachineSpace().Value);
        Assert.AreEqual(27, grid.BlocksCount());
        Assert.AreEqual(finishingBlock, grid.Blocks().FirstOrDefault(b => b == finishingBlock));
    }

    [TestMethod]
    public void FuzzyFailToFind()
    {
        var nothing = new Material(1, 1);
        var space = new GridBox(new Box(new Position(10, 10, 10), new Position(42, 12, 42)));
        foreach (var block in space.Blocks())
        {
            this.Provider.SetMaterial(this.Filler, block);
        }
        var randoms = new RandomBlocks(space);
        foreach (var i in Enumerable.Range(0, 100))
        {
            var finishingBlock = randoms.First();
            var checker = new FilledBoxChecker(this.Surveyor, finishingBlock, new Position(33, 3, 33), this.Filler);
            var emptyCount = randoms.Generator.Next(15) + 1;
            var empties = randoms.Where(b => b != finishingBlock).Take(emptyCount).ToList();
            foreach(var empty in empties)
            {
                this.Provider.SetMaterial(nothing, empty);
            }
            Assert.AreEqual(null, checker.MachineSpace());
            foreach(var empty in empties)
            {
                this.Provider.SetMaterial(this.Filler, empty);
            }
        }
    }

    [TestMethod]
    public void FuzzySuccessFind()
    {
        var nothing = new Material(1, 1);
        var space = new GridBox(new Box(new Position(10, 10, 10), new Position(28, 36, 28)));
        foreach (var block in space.Blocks())
        {
            this.Provider.SetMaterial(this.Filler, block);
        }
        var size = new Position(9, 13, 9);
        var toFind = new Box(new Position(15, 17, 15), new Position(23, 29, 23));
        var randoms = new RandomBlocks(space);
        foreach (var i in Enumerable.Range(0, 100))
        {
            var finishingBlock = randoms
                .Where(b => toFind.Outfits(new Box(b, b)))
                .First();

            var checker = new FilledBoxChecker(this.Surveyor, finishingBlock, size, this.Filler);

            var emptyCount = randoms.Generator.Next(15) + 1;
            var empties = randoms.Where(b => !toFind.Outfits(new Box(b, b))).Take(emptyCount).ToList();
            foreach(var empty in empties)
            {
                this.Provider.SetMaterial(nothing, empty);
            }

            Assert.IsTrue(null != checker.MachineSpace());

            foreach(var empty in empties)
            {
                this.Provider.SetMaterial(this.Filler, empty);
            }
        }
    }
}

class RandomBlocks : IEnumerable<Position>
{
    public RandomBlocks(GridBox grid)
    {
        this.Positions = grid.Blocks().ToList();
    }

    public IEnumerator<Position> GetEnumerator()
    {
        while(true)
        {
            yield return this.Positions[this.Generator.Next(this.Positions.Count())];
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public IEnumerable<Position> TakeSome(int some)
    {
        var amount = this.Generator.Next(some) + 1;
        return this.Take(amount);
    }

    List<Position> Positions;
    public System.Random Generator = new System.Random();
}
}