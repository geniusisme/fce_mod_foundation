using FortressCraft.ModFoundation.Block;

namespace FortressCraft.ModFoundation
{
public class BlockSurveyor
{
    public BlockSurveyor(SegmentProvider segments)
    {
        this.Segments = segments;
    }

    public BlockSurveyor(MachineEntity anchor): this(new NeighbourhoodSegmentProvider(anchor))
    {}

    public BlockSurveyor(FrustrumBase frustrum): this(new FrustrumSegmentProvider(frustrum))
    {}

    public SurveyOperation Look()
    {
        return new SurveyOperation(this.Segments);
    }

    SegmentProvider Segments;
}

public interface SegmentProvider
{
    Segment GetSegment(Position position);
}

class NeighbourhoodSegmentProvider : SegmentProvider
{
    public NeighbourhoodSegmentProvider(MachineEntity anchor)
    {
        this.Anchor = anchor;
    }

    public Segment GetSegment(Position position)
    {
        return this.Anchor.AttemptGetSegment(position.X, position.Y, position.Z);
    }

    MachineEntity Anchor;
}

class FrustrumSegmentProvider : SegmentProvider
{
    public FrustrumSegmentProvider(FrustrumBase frustrum)
    {
        this.Frustrum = frustrum;
    }

    public Segment GetSegment(Position position)
    {
        var segment = this.Frustrum.GetSegment(position.X, position.Y, position.Z);
        if (segment?.IsSegmentInAGoodState() ?? false)
        {
            return segment;
        }
        return null;
    }

    FrustrumBase Frustrum;
}

public class SurveyOperation
{
    public SurveyOperation(SegmentProvider segments)
    {
        this.Segments = segments;
    }

    public SurveyOperation At(Position position)
    {
        this.X = position.X;
        this.Y = position.Y;
        this.Z = position.Z;
        this.Segment = this.Segments.GetSegment(position);
        if (this.Segment != null)
        {
            this.IsOk = true;
        }
        return this;
    }

    public SurveyOperation WhereBlockIs(ushort materialType)
    {
        if (this.IsOk)
        {
            this.IsOk = Segment.GetCube(this.X, this.Y, this.Z) == materialType;
        }
        return this;
    }

    public Material? Block()
    {
        if (this.IsOk)
        {
            return new Material(
                Segment.GetCube(this.X, this.Y, this.Z),
                this.Segment.GetCubeData(this.X, this.Y, this.Z).mValue
            );
        }
        return null;
    }

    public SurveyOperation WhereBlockIs(Material material)
    {
        var operation = this.WhereBlockIs(material.Type);
        if (operation.IsOk)
        {
            operation.IsOk = operation.Segment.GetCubeData(operation.X, operation.Y, operation.Z).mValue == material.Value;
        }
        return operation;
    }

    public T For<T>() where T : class
    {
        if (!this.IsOk)
        {
            return null;
        }
        var entity = this.Segment.SearchEntity(this.X, this.Y, this.Z);
        if (entity.mbDelete)
        {
            return null;
        }
        return entity as T;
    }

    bool IsOk = false;
    readonly SegmentProvider Segments;
    Segment Segment;
    long X;
    long Y;
    long Z;
}

}
