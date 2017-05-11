using System.Collections.Generic;
using System.Linq;

namespace FortressCraft.ModFoundation
{
public static class HashUtil
{
    static readonly int Prime = 486187739;

    public static int CombineHash<T>(this int hash, T them)
    {
        unchecked
        {
            return them.GetHashCode() + Prime * hash;
        }
    }

    public static int CombineHash<T1, T2>(this T1 us, T2 them)
    {
        return us.GetHashCode().CombineHash(them);
    }

    public static int CombineHash<T>(this IEnumerable<T> ts)
    {
        return 0.CombineHash(ts);
    }

    public static int CombineHash<T>(this int hash, IEnumerable<T> ts)
    {
        return ts.Aggregate(hash, (h, t) => h.CombineHash(t));
    }
}
}
