using System;
using System.Linq;
using System.Collections.Generic;

namespace FortressCraft.ModFoundation
{
public static class EnumeratorUtil
{
    public static IEnumerable<T> Once<T>(this T us)
    {
        yield return us;
    }

    public static IEnumerable<Result> Zip<Us, Them, Result>(
        this IEnumerable<Us> us,
        IEnumerable<Them> them,
        Func<Us, Them, Result> func)
    {
        var usIt = us.GetEnumerator();
        var themIt = them.GetEnumerator();
        {
            while (usIt.MoveNext() && themIt.MoveNext())
            {
                yield return func(usIt.Current, themIt.Current);
            }
        }
    }

    public struct IndexedValue<T>
    {
        public IndexedValue(T value, int index)
        {
            this.Value = value;
            this.Index = index;
        }
        public readonly T Value;
        public readonly int Index;
    }

    public static IEnumerable<IndexedValue<T>> Index<T>(this IEnumerable<T> ts)
    {
        int index = 0;
        foreach(var t in ts)
        {
            yield return new IndexedValue<T>(t, index++);
        }
    }

    public static IEnumerable<T> Endless<T>(this T item)
    {
        for(;;) yield return item;
    }

    public static IEnumerable<T> Loop<T>(this IEnumerable<T> seq)
    {
        return seq.Endless().SelectMany(x => x);
    }

    public static bool AllTrue(this IEnumerable<bool> seq)
    {
        return seq.All((b) => b);
    }

    public static bool NoneTrue(this IEnumerable<bool> seq)
    {
        return seq.All((b) => !b);
    }

    public static T? Find<T>(this IEnumerable<T> ts, Func<T, bool> pred) where T: struct
    {
        try
        {
            return ts.First(pred);
        }
        catch(InvalidOperationException)
        {
            return null;
        }
    }

    public static IEnumerable<U> SelectValues<T, U>(this IEnumerable<T> seq, Func<T, U?> map) where U: struct
    {
        return seq.Select(map).Where((v) => v.HasValue).Select((v) => v.Value);
    }

    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> seq)
    {
        return new HashSet<T>(seq) ;
    }
}
}
