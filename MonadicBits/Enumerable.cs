using System;
using System.Collections.Generic;

namespace MonadicBits
{
    [Obsolete("use MonadicEnumerable instead")]
    public static class Enumerable
    {
        public static IEnumerable<T> Return<T>(T value)
        {
            yield return value;
        }
    }
    
    public static class MonadicEnumerable
    {
        public static IEnumerable<T> Return<T>(T value)
        {
            yield return value;
        }

        public static IEnumerable<T> ToEnumerable<T>(this T @this) =>
            Return(@this);
    }
}