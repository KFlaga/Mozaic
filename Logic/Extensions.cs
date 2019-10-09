using System.Collections.Generic;

namespace MozaicLand
{
    public static class Extensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T[,] target)
        {
            foreach (var item in target)
                yield return (T)item;
        }
    }
}
