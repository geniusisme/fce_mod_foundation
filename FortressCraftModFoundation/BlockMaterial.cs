using System;

using static FortressCraft.ModFoundation.HashUtil;

namespace FortressCraft.ModFoundation.Block
{
/// represents substance making a block
/// uniquely identified by cube type + cube value (see modding documentation)
/// all stationary objects in the game are make from blocks, which differ by their material
public struct Material
{
    /// look up registered entries from TerrainData.xml
    public Material(String name)
    {
        int id;
        ushort type;
        ushort value;
        MaterialData.GetItemIdOrCubeValues(name, out id, out type, out value);
        if (type == 0)
        {
            throw new IncorrectKeyForBlockTypeOrValueException();
        }
        this.Type = type;
        this.Value = value;
    }

    public Material(ushort type = 0, ushort value = 0)
    {
        this.Type = type;
        this.Value = value;
    }

    public readonly ushort Type;
    public readonly ushort Value;

    public override bool Equals(Object them)
    {
        if (!(them is Material))
        {
            return false;
        }

        return this == (Material) them;
    }

    public override int GetHashCode()
    {
        return this.Type.CombineHash(this.Value);
    }

    public override String ToString()
    {
        return "type: " + this.Type + " value: " + this.Value;
    }

    static public bool operator ==(Material us, Material them)
    {
        return us.Type == them.Type && us.Value == them.Value;
    }

    static public bool operator !=(Material us, Material them)
    {
        return !(us == them);
    }
}

public class IncorrectKeyForBlockTypeOrValueException : Exception {}
}
