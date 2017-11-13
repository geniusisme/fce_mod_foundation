using System.Collections.Generic;
using System.Linq;
using FortressCraft.ModFoundation;
using FortressCraft.ModFoundation.Block;

namespace ModFoundationTest
{
class SegmentProviderStub : SegmentProvider
{
    Dictionary<Position, Segment> Segments = new Dictionary<Position, Segment>();

    public Segment GetSegment(Position position)
    {       
        var segPos = this.SegmentPosition(position);
        if (this.Segments.ContainsKey(segPos))
        {
            return this.Segments[segPos];
        }
        else
        {
            return this.AddSegment(segPos);
        }
    }

    public void AddEntity(SegmentEntity entity, Material material)
    {
        var segment = this.GetSegment(new Position(entity));
        if (segment.mEntities[(int) entity.mType] == null)
            segment.mEntities[(int) entity.mType] = new List<SegmentEntity>();
        segment.mEntities[(int) entity.mType].Add(entity);
        this.SetMaterial(material, new Position(entity));
    }

    public void SetMaterial(Material material, Position position)
    {
        var segment = this.GetSegment(position);
        var local = new Position(position.Select(c => c % 16));
        segment.SetCubeTypeNoChecking((int)local.X, (int)local.Y, (int)local.Z, material.Type, material.Value);
    }

    Position SegmentPosition(Position entityPosition)
    {
        return new Position(entityPosition.Select(c => c - c % 16));
    }

    Segment AddSegment(Position position)
    {
        var segment = new Segment(position.X, position.Y, position.Z);
        segment.mbIsEmpty = true;
        segment.PopulateEmptySegment();
        segment.mEntities = new List<SegmentEntity>[(int) eSegmentEntity.Num];
        segment.mbInitialGenerationComplete = true;
        this.Segments[position] = segment;
        return segment;
    }
}
}
