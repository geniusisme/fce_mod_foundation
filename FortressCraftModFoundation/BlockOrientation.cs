using System;
using System.Collections.Generic;
using System.Linq;

namespace FortressCraft.ModFoundation.Block
{
/// This struct provides info about orientation in space, including all possible tilts and rotations
/// Keep in mind, it is block orientation, and can only orient along cardinal directions
public struct Orientation
{
    public Orientation(Direction newX, Direction newY)
    {
        if (newX.IsAlong(newY))
        {
            throw new ArgumentException("directions must be perpendicular");
        }

        var newZ = newY.RotateCcw(newX);
        this.Rotations = new Direction[]{ newX, newY, newZ };
    }

    public Direction ApplyTo(Direction direction)
    {
        if (direction.Index > 2)
        {
            return Rotations[direction.Negate().Index].Negate();
        }
        return Rotations[direction.Index];
    }

    public byte Flags()
    {
        var facing = this.Rotations[1].Negate();
        var flags = facing.Flags();
        var turns = (byte) 0;
        if (facing.IsAlong(Direction.PlusX()))
        {
            var newZ = this.Rotations[2];
            turns = (byte) Revolves(Direction.PlusZ(), facing)
                .Index()
                .First(di => di.Value == newZ)
                .Index;
        }
        else
        {
            var newX = this.Rotations[0];
            turns = (byte) Revolves(Direction.PlusX(), facing)
                .Index()
                .First(di => di.Value == newX)
                .Index;

        }
        return (byte) ((turns <<  6) | flags);
    }

    static public Orientation Identity()
    {
        return new Orientation(Direction.PlusX(), Direction.PlusY());
    }

    static public Orientation MakeFromFlags(byte flags)
    {
        var facing = Direction.MakeFromFlags(flags);
        var Y = facing.Negate();
        var turns = flags >> 6;
        if (facing.IsAlong(Direction.PlusX()))
        {
            var Z = Revolves(Direction.PlusZ(), facing).Take(turns + 1).Last();
            var X = Z.RotateCcw(Y);
            return new Orientation(X, Y);
        }
        else
        {
            var X = Revolves(Direction.PlusX(), facing).Take(turns + 1).Last();
            return new Orientation(X, Y);
        }
    }

    static IEnumerable<Direction> Revolves(Direction revolving, Direction axis)
    {
        while (true)
        {
            yield return revolving;
            revolving = revolving.RotateCcw(axis);
        }
    }

    Direction[] Rotations;
}
}