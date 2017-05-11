using System.Collections.Generic;

namespace FortressCraft.ModFoundation.Block
{
public struct Direction
{
    public readonly Position Shift;
    public readonly int Index;

    public Direction(Position shift, int index)
    {
        this.Shift = shift;
        this.Index = index;
    }

    public Direction Negate()
    {
        return new Direction(new Position(0, 0, 0) - this.Shift, (this.Index + 3) % 6);
    }

    public static Direction PlusX()
    {
        return MakeDirection(1, 0, 0, 0);
    }

    public static Direction PlusY()
    {
        return MakeDirection(0, 1, 0, 1);
    }

    public static Direction PlusZ()
    {
        return MakeDirection(0, 0, 1, 2);
    }

    public static Direction MinusX()
    {
        return MakeDirection(-1, 0, 0, 3);
    }

    public static Direction MinusY()
    {
        return MakeDirection(0, -1, 0, 4);
    }

    public static Direction MinusZ()
    {
        return MakeDirection(0, 0, -1, 5);
    }

    public static IEnumerable<Direction> All()
    {
        yield return PlusX();
        yield return PlusY();
        yield return PlusZ();
        yield return MinusX();
        yield return MinusY();
        yield return MinusZ();
    }

    public static IEnumerable<Direction> Cardinal()
    {
        yield return PlusX();
        yield return PlusY();
        yield return PlusZ();
    }

    static Direction MakeDirection(long x, long y, long z, int index)
    {
        return new Direction(new Position(x, y, z), index);
    }
}
}
