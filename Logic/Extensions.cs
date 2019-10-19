using System.Collections.Generic;
using System;
using System.Drawing;

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

        public static void Fill<T>(this T[,] t, T value)
        {
            for(int r = 0; r < t.Rows(); ++r)
            {
                for (int c = 0; c < t.Cols(); ++c)
                {
                    t[r, c] = value;
                }
            }
        }

        public static void Fill<T>(this T[,] t, Func<int, int, T> generator)
        {
            for (int r = 0; r < t.Rows(); ++r)
            {
                for (int c = 0; c < t.Cols(); ++c)
                {
                    t[r, c] = generator(r, c);
                }
            }
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

        public static PointF Sub(this PointF a, PointF b)
        {
            return new PointF(a.X - b.X, a.Y - b.Y);
        }

        public static PointF Add(this PointF a, PointF b)
        {
            return new PointF(a.X + b.X, a.Y + b.Y);
        }

        public static float Length(this PointF a)
        {
            return (float)Math.Sqrt(a.X * a.X + a.Y * a.Y);
        }

        public static float DistanceTo(this PointF a, PointF b)
        {
            return a.Sub(b).Length();
        }
    }
}
