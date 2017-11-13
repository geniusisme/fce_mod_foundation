using UnityEngine;

namespace FortressCraft.ModFoundation
{
    /// class for machines which want build up on existing in-game machines
    /// this class takes care for graphics, if you want to reuse other functionality, you need to add code youself
public class OverloadedMachine<V> : MachineEntity
    where V : MachineEntity
{
    /// original in-game machine, available for sub-class
    protected V Vanilla;

    /// override this instead of low frequency update.
    /// You only need to do specific to your machine stuff, generic stuff is done for you.
    public virtual void Update(float timeDelta) {}

    /// pass vanilla object to wrap, construct it from the same parameters
    public OverloadedMachine(ModCreateSegmentEntityParameters parameters, V vanilla)
       : base(parameters)
    {
        this.Vanilla = vanilla;
        this.mbNeedsLowFrequencyUpdate = true;
        this.mbNeedsUnityUpdate = true;
    }

    public override void LowFrequencyUpdate()
    {
        this.Vanilla.UpdatePlayerDistanceInfo();
        this.Update(LowFrequencyThread.mrPreviousUpdateTimeStep);
        this.MarkDirtyDelayed();
    }

    public override void UnityUpdate()
    {
        this.Vanilla.UnityUpdate();
    }

    public override void UnitySuspended()
    {
        this.Vanilla.UnitySuspended();
    }

    public override HoloMachineEntity CreateHolobaseEntity(Holobase holobase)
    {
        return this.Vanilla.CreateHolobaseEntity(holobase);
    }

    public override void HolobaseUpdate(Holobase holobase, HoloMachineEntity holoMachineEntity)
    {
        this.Vanilla.HolobaseUpdate(holobase, holoMachineEntity);
    }

    public override void SpawnGameObject()
    {
        this.Vanilla.SpawnGameObject();
        this.mWrapper = this.Vanilla.mWrapper;
    }

    public override void DropGameObject()
    {
        this.Vanilla.DropGameObject();
    }

    public override bool ShouldSave()
    {
        return true;
    }

    public override bool ShouldNetworkUpdate()
    {
        return ShouldSave();
    }

    public override void OnDelete()
    {
        this.Vanilla.OnDelete();
    }

    public override void CleanUp()
    {
        this.Vanilla.CleanUp();
    }

    public override void BuiltOn(int newType)
    {
        this.Vanilla.BuiltOn(newType);
        base.BuiltOn(newType);
    }

    public override void OnUpdateRotation(byte newFlags)
    {
        this.Vanilla.OnUpdateRotation(newFlags);
    }

    public override void GetTransform(out Vector3 position, out Quaternion rotation, out Vector3 scale)
    {
         this.Vanilla.GetTransform(out position, out rotation, out scale);
    }

    public override bool HasLongRangeGraphics()
    {
        return this.Vanilla.HasLongRangeGraphics();
    }

    public override void UpdateRayCastVis()
    {
        this.Vanilla.UpdateRayCastVis();
    }

    public override void UpdatePlayerDistanceInfo()
    {
        this.Vanilla.UpdatePlayerDistanceInfo();
    }
}
}
