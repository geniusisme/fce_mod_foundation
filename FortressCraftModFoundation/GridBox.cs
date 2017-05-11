using System.Collections.Generic;
using System.Linq;
using FortressCraft.ModFoundation.Block;


namespace FortressCraft.ModFoundation.Multiblock
{
/// class for enumerating individual blocks related to box
public class GridBox
{
    public readonly Box Box;

    public GridBox(Box box)
    {
        this.Box = box;
    }

    /// enumerate blocks on sides of box
    /// i.e. blocks which touch sides, but are not inside of the box
    public IEnumerable<Position> Sides()
    {
        return
            SidePlusX().Concat(
            SidePlusY()).Concat(
            SidePlusZ()).Concat(
            SideMinusX()).Concat(
            SideMinusY()).Concat(
            SideMinusZ());
    }

    /// enumerate blocks on individual side of the box, corresponding to direction
    /// i.e. blocks which touch sides, but are not inside of the box
    public IEnumerable<Position> Side(Direction direction)
    {
        switch (direction.Index)
        {
            case 0: return SidePlusX();
            case 1: return SidePlusY();
            case 2: return SidePlusZ();
            case 3: return SideMinusX();
            case 4: return SideMinusY();
            case 5: return SideMinusZ();
        }
        return null;
    }

    /// enumerate all blocks inside the box
    public IEnumerable<Position> Blocks()
    {
        for (var x = this.Box.Min.X; x <= this.Box.Max.X; ++x)
        {
            for (var y = this.Box.Min.Y; y <= this.Box.Max.Y; ++y)
            {
                for (var z = this.Box.Min.Z; z <= this.Box.Max.Z; ++z)
                {
                    yield return new Position(x, y, z);
                }
            }
        }
    }

    /// count of all blocks returned by Blocks()
    public int BlocksCount()
    {
        return (int) (this.Box.Size().X * this.Box.Size().Y * this.Box.Size().Z);
    }

    /// count of all blocks returned by Sides()
    public int SidesCount()
    {
        var width = this.Box.Size().X;
        var depth = this.Box.Size().Y;
        var height = this.Box.Size().Z;
        return (int) (2 * width * height + 2 * width * depth + 2 * depth * height);
    }

    IEnumerable<Position> SidePlusX()
    {
        for (var z = this.Box.Min.Z; z <= this.Box.Max.Z; ++z)
        {
            for (var y = this.Box.Min.Y; y <= this.Box.Max.Y; ++y)
            {
                yield return new Position(this.Box.Max.X + 1, y, z);
            }
        }
    }

    IEnumerable<Position> SidePlusY()
    {
        for (var x = this.Box.Min.X; x <= this.Box.Max.X; ++x)
        {
            for (var z = this.Box.Min.Z; z <= this.Box.Max.Z; ++z)
            {
                yield return new Position(x, this.Box.Max.Y + 1, z);
            }
        }
    }

    IEnumerable<Position> SidePlusZ()
    {
        for (var x = this.Box.Min.X; x <= this.Box.Max.X; ++x)
        {
            for (var y = this.Box.Min.Y; y <= this.Box.Max.Y; ++y)
            {
                yield return new Position(x, y, this.Box.Max.Z + 1);
            }
        }
    }

    IEnumerable<Position> SideMinusX()
    {
        for (var z = this.Box.Min.Z; z <= this.Box.Max.Z; ++z)
        {
            for (var y = this.Box.Min.Y; y <= this.Box.Max.Y; ++y)
            {
                yield return new Position(this.Box.Min.X - 1, y, z);
            }
        }
    }

    IEnumerable<Position> SideMinusY()
    {
        for (var x = this.Box.Min.X; x <= this.Box.Max.X; ++x)
        {
            for (var z = this.Box.Min.Z; z <= this.Box.Max.Z; ++z)
            {
                yield return new Position(x, this.Box.Min.Y - 1, z);
            }
        }
    }

    IEnumerable<Position> SideMinusZ()
    {
        for (var x = this.Box.Min.X; x <= this.Box.Max.X; ++x)
        {
            for (var y = this.Box.Min.Y; y <= this.Box.Max.Y; ++y)
            {
                yield return new Position(x, y, this.Box.Min.Z - 1);
            }
        }
    }
}
}
