using System.Collections.Generic;
using System.Linq;
using FortressCraft.ModFoundation.Block;
using FortressCraft.ModFoundation.Multiblock;

using static FortressCraft.ModFoundation.EnumeratorUtil;

namespace FortressCraft.ModFoundation
{
public class AdjacentsSurveyor
{
	IEnumerator<IndexedValue<Position>> Sides;
	BlockSurveyor Surveyor;
	List<SegmentEntity> Adjacents;

	public AdjacentsSurveyor(Box box, MachineEntity anchor)
	{
		var grid = new GridBox(box);
		this.Sides = grid.Sides().Index().Loop().GetEnumerator();
		this.Surveyor = new BlockSurveyor(anchor);
		this.Adjacents = Enumerable.Repeat<SegmentEntity>(null, grid.SidesCount()).ToList();
	}

	public void Update()
	{
		var sides = this.Sides;
		sides.MoveNext();
		var savedEntity = Adjacents[sides.Current.Index];
		if (!IsGood(savedEntity))
		{
			this.Adjacents[sides.Current.Index] = this.Surveyor
				.Look()
				.At(this.Sides.Current.Value)
				.For<SegmentEntity>();
		}
	}

	static bool IsGood(SegmentEntity entity)
	{
		return entity != null && !entity.mbDelete;
	}

	public IEnumerable<T> Surveyed<T>() where T : class
	{
		return this.Adjacents
			.Where((x) => IsGood(x))
			.Select((x) => x as T)
			.Where((x) => x != null);
	}
}
}
