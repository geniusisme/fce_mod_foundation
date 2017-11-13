using System;
using System.Collections.Generic;
using System.Linq;

namespace FortressCraft.ModFoundation.Block
{
public struct Position : IEnumerable<long>
{  //TODO: multiply by scalar
    public long X;
    public long Y;
    public long Z;

    public Position(long x, long y, long z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public Position(ModCreateSegmentEntityParameters parameters)
        : this(parameters.X, parameters.Y, parameters.Z)
    {}

    public Position(ModCheckForCompletedMachineParameters parameters)
        : this(parameters.X, parameters.Y, parameters.Z)
    {}

    public Position(SegmentEntity entity)
        : this(entity.mnX, entity.mnY, entity.mnZ)
    {}

    public Position(IEnumerable<long> sequence)
    {
        var iterator = sequence.GetEnumerator();
        iterator.MoveNext();
        this.X = iterator.Current;
        iterator.MoveNext();
        this.Y = iterator.Current;
        iterator.MoveNext();
        this.Z = iterator.Current;
    }

    public override bool Equals(Object them)
    {
        if (!(them is Position))
        {
            return false;
        }

        return this == (Position) them;
    }

    public override int GetHashCode()
    {
        return HashUtil.CombineHash(this);
    }

    public IEnumerator<long> GetEnumerator()
    {
        yield return this.X;
        yield return this.Y;
        yield return this.Z;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public long this[int coord]
    {
        get
        {
            switch(coord)
            {
                case 0: return this.X;
                case 1: return this.Y;
                case 2: return this.Z;
            }
            throw new ArgumentOutOfRangeException("coordinate of position");
        }

        set
        {
            switch(coord)
            {
                case 0: this.X = value; return;
                case 1: this.Y = value; return;
                case 2: this.Z = value; return;
            }
            throw new ArgumentOutOfRangeException("coordinate of position");
        }
    }

    public override String ToString()
    {
        return "x: " + this.X + " y: " + this.Y + " z: " + this.Z;
    }

    public static Position operator +(Position us, Position them)
    {
        return new Position(us.X + them.X, us.Y + them.Y, us.Z + them.Z);
    }

    public static Position operator -(Position us, Position them)
    {
        return new Position(us.X - them.X, us.Y - them.Y, us.Z - them.Z);
    }

    public static Position operator *(Position us, int them)
    {
        return new Position(us.Select(c => c * them));
    }

    public static Position operator /(Position us, int them)
    {
        return new Position(us.Select(c => c / them));
    }

    public static bool operator ==(Position us, Position them)
    {
        return (us.X == them.X) && (us.Y == them.Y) && (us.Z == them.Z);
    }

    public static bool operator !=(Position us, Position them)
    {
        return !(us == them);
    }
}

public static class BlockPositionUtil
{
    public static Position ToPosition(this IEnumerable<long> seq)
    {
        return new Position(seq);
    }
}
}
