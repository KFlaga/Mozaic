using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
