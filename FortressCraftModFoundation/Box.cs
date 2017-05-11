using System;
using System.Linq;

using static System.Math;
using static FortressCraft.ModFoundation.EnumeratorUtil;

namespace FortressCraft.ModFoundation.Block
{
public struct Box
{
    public readonly Position Max;
    public readonly Position Min;

    public Box(Position lhsCorner, Position rhsCorner)
    {
        this.Min = new Position(Min(lhsCorner.X, rhsCorner.X), Min(lhsCorner.Y, rhsCorner.Y), Min(lhsCorner.Z, rhsCorner.Z));
        this.Max = new Position(Max(lhsCorner.X, rhsCorner.X), Max(lhsCorner.Y, rhsCorner.Y), Max(lhsCorner.Z, rhsCorner.Z));
    }

    public Position Size()
    {
        return new Position(Max.X - Min.X + 1, Max.Y - Min.Y + 1, Max.Z - Min.Z + 1);
    }

    public Position Center()
    {
        var sum = this.Min + this.Max;
        return new Position(sum.X / 2, sum.Y / 2, sum.Z / 2);
    }

    // TODO: add size of extension
    public Box Extended(Direction direction)
    {
        if (direction.Index < 3)
        {
            return new Box(this.Min, this.Max + direction.Shift);
        }
        else
        {
            return new Box(this.Min + direction.Shift, this.Max);
        }
    }

    /// this is strict boxes relation
    /// strict in a sense that equal boxes do not contain each other
    public BoxRelation Relation(Box them)
    {
        var result = BoxRelation.Equals;
        foreach(var coord in Enumerable.Range(0, 3))
        {
            var relation = Relation(this.Min[coord], this.Max[coord], them.Min[coord], them.Max[coord]);
            if (relation == BoxRelation.Apart)
            {
                return relation;
            }
            if (result == BoxRelation.Equals)
            {
                result = relation;
                continue;
            }
            if (relation == result)
            {
                continue;
            }
            if (relation == BoxRelation.Equals)
            {
                continue;
            }
            result = BoxRelation.Intersects;
        }
        return result;
    }

    BoxRelation Relation(long usMin, long usMax, long themMin, long themMax)
    {
        if (usMin < themMin && usMax >= themMin)
        {
            if (usMax >= themMax)
            {
                return BoxRelation.Contains;
            }
            else
            {
                return BoxRelation.Intersects;
            }
        }
        else if(themMin < usMin && themMax >= usMin)
        {
            if (themMax >= usMax)
            {
                return BoxRelation.Contained;
            }
            else
            {
                return BoxRelation.Intersects;
            }
        }
        else if (usMin == themMin)
        {
            if (usMax > themMax)
            {
                return BoxRelation.Contains;
            }
            else if (usMax < themMax)
            {
                return BoxRelation.Contained;
            }
            else
            {
                return BoxRelation.Equals;
            }
        }
        else
        {
            return BoxRelation.Apart;
        }
    }

    public bool FitsIn(Box them)
    {
        return them.Outfits(this);
    }

    public bool FitsIn(Position size)
    {
        return this.Size().Zip(size, (t, s) => t <= s).AllTrue();
    }

    public bool Outfits(Box them)
    {
        return
            this.Min.Zip(them.Min, (us, they) => us <= they).AllTrue() &&
            this.Max.Zip(them.Max, (us, they) => us >= they).AllTrue();
    }

    public bool Outfits(Position size)
    {
        return this.Size().Zip(size, (t, s) => t >= s).AllTrue();
    }

    public Box? Intersect(Box them)
    {
        var min = this.Min.Zip(them.Min, (us, they) => Max(us, they)).ToPosition();
        var max = this.Max.Zip(them.Max, (us, they) => Min(us, they)).ToPosition();
        if (min.Zip(max, (mi, ma) => mi <= ma).AllTrue())
        {
            return new Box(min, max);
        }
        else
        {
            return null;
        }
    }

    public override int GetHashCode()
    {
        return HashUtil.CombineHash(this.Min, this.Max);
    }

    public override bool Equals(Object them)
    {
        if (!(them is Box))
        {
            return false;
        }

        return this == (Box) them;
    }

    public override String ToString()
    {
        return "min: " + this.Min + " max: " + this.Max;
    }

    public static bool operator ==(Box us, Box them)
    {
        return us.Min == them.Min && us.Max == them.Max;
    }

    public static bool operator !=(Box us, Box them)
    {
        return !(us == them);
    }
}

public enum BoxRelation
{
    Equals,
    Contains,
    Contained,
    Intersects,
    Apart,
}
}
