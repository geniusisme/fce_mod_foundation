namespace FortressCraft.ModFoundation
{
public static class MathUtil
{
    public static T Clamp<T>(this T t, T min, T max)
        where T : System.IComparable<T>
    {
         if (t.CompareTo(max) > 0)
         {
            return max;
         }
         if (t.CompareTo(min) < 0)
         {
            return min;
         }
         return t;
    }
}
}