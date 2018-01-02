using System;
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

    public Direction RotateCcw(Direction axis)
    {
        if (this.IsAlong(axis))
        {
            return this;
        }
        var newShift = new Position (
            axis.Shift.Y * this.Shift.Z - axis.Shift.Z * this.Shift.Y,
            axis.Shift.Z * this.Shift.X - axis.Shift.X * this.Shift.Z,
            axis.Shift.X * this.Shift.Y - axis.Shift.Y * this.Shift.X
        );
        var uniqueValue = (newShift.X + 2) + (newShift.Y + 2) * 2 + (newShift.Z + 2) * 4;
        switch (uniqueValue)
        {
            case 13: return MinusX();
            case 15: return PlusX();
            case 12: return MinusY();
            case 16: return PlusY();
            case 10: return MinusZ();
            case 18: return PlusZ();
            default: return PlusX();
        }
    }

    public bool IsAlong(Direction them)
    {
        return this.Index % 3 == them.Index % 3;
    }

    public byte Flags()
    {
        switch (this.Index)
        {
            case 0: return 16;
            case 1: return 2;
            case 2: return 8;
            case 3: return 32;
            case 4: return 1;
            case 5: return 4;
            default: return 0;
        }
    }

    public static Direction Up()
    {
        return PlusY();
    }

    public static Direction Down()
    {
        return MinusY();
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

    public static Direction MakeFromFlags(byte directionFlags)
    {
        switch(directionFlags)
        {
            case 1: return MinusY();
            case 2: return PlusY();
            case 4: return MinusZ();
            case 8: return PlusZ();
            case 16: return PlusX();
            case 32: return MinusX();
        }
        return MinusY();
    }

    public override String ToString()
    {
        switch(this.Index)
        {
            case 0: return "Plus X";
            case 1: return "Plus Y";
            case 2: return "Plus Z";
            case 3: return "Minus X";
            case 4: return "Minus Y";
            case 5: return "Minus Z";
        }
        return "direction what?";
    }

    public override bool Equals(object them)
    {
        if (!(them is Direction))
        {
            return false;
        }

        return this == (Direction) them;
    }

    public static bool operator ==(Direction us, Direction them)
    {
        return us.Index == them.Index;
    }

    public static bool operator !=(Direction us, Direction them)
    {
        return !(us == them);
    }

    public override int GetHashCode()
    {
        return this.Index.GetHashCode();
    }

    static Direction MakeDirection(long x, long y, long z, int index)
    {
        return new Direction(new Position(x, y, z), index);
    }
}
}
