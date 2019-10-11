using System.Collections.Generic;
using System;

namespace MozaicLand
{
    public static class Extensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T[,] target)
        {
            foreach (var item in target)
                yield return (T)item;
        }

        public static int Rows<T>(this T[,] t)
        {
            return t.GetLength(0);
        }

        public static int Cols<T>(this T[,] t)
        {
            return t.GetLength(1);
        }

        private struct AlwaysGreater : IComparable
        {
            public int CompareTo(object obj)
            {
                return 1;
            }
        }

        private struct AlwaysLower : IComparable
        {
            public int CompareTo(object obj)
            {
                return -1;
            }
        }
        
        public static T MinElement<T>(this IEnumerable<T> enumerable, Func<T, IComparable> selector)
        {
            IComparable minValue = new AlwaysGreater();
            T minElement = default(T);
            foreach(T item in enumerable)
            {
                IComparable value = selector(item);
                if(minValue.CompareTo(value) > 0)
                {
                    minElement = item;
                    minValue = value;
                }
            }
            return minElement;
        }
        
        public static T MaxElement<T>(this IEnumerable<T> enumerable, Func<T, IComparable> selector)
        {
            IComparable maxValue = new AlwaysLower();
            T maxElement = default(T);
            foreach (T item in enumerable)
            {
                IComparable value = selector(item);
                if (maxValue.CompareTo(value) < 0)
                {
                    maxElement = item;
                    maxValue = value;
                }
            }
            return maxElement;
        }
    }
}
