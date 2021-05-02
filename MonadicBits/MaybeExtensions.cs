using System;
using System.Collections.Generic;
using System.Linq;
using static MonadicBits.Functional;

namespace MonadicBits
{
    public static class MaybeExtensions
    {
        public static Maybe<T> Just<T>(this T value)
        {
            if (value == null) throw new ArgumentNullException(
                nameof(value), 
                "cannot create just from null value");
            return value;
        }

        public static Maybe<T> JustNotNull<T>(this T value) =>
            value == null ? Nothing : value.Just();

        public static Maybe<T> FirstOrNothing<T>(this IEnumerable<T> source) =>
            source.Select(value => value.Just()).DefaultIfEmpty(Nothing).First();

        public static Maybe<T> JustWhen<T>(this T value, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            
            return predicate(value) ? value.Just() : Nothing;
        }
    }
}