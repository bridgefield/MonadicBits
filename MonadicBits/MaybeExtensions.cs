using System;
using System.Collections.Generic;
using System.Linq;

namespace MonadicBits
{
    public static class MaybeExtensions
    {
        public static Maybe<T> Just<T>(this T value) => Maybe<T>.Just(value);

        public static Maybe<T> JustNotNull<T>(this T value) =>
            value == null ? Maybe<T>.Nothing() : Maybe<T>.Just(value);

        public static Maybe<T> FirstOrNothing<T>(this IEnumerable<T> source) =>
            source.Select(value => value.Just()).DefaultIfEmpty(Maybe<T>.Nothing()).First();

        public static Maybe<T> JustWhen<T>(this T value, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            
            return predicate(value) ? Maybe<T>.Just(value) : Maybe<T>.Nothing();
        }
    }
}