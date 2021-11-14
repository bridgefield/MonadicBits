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
            if (value == null)
                throw new ArgumentNullException(
                    nameof(value),
                    "cannot create just from null value");
            return value;
        }

        public static Maybe<T> JustNotNull<T>(this T value) =>
            value?.Just() ?? Nothing;

        public static Maybe<T> ToMaybe<T>(this T? source) where T : struct =>
            source?.Just() ?? Nothing;

        public static T? ToNullable<T>(this Maybe<T> source) where T : struct =>
            source.Match<T?>(j => j, () => null);

        public static Maybe<T> Or<T>(this Maybe<T> source, Func<Maybe<T>> alternative) =>
            source.Match(j => j, alternative);

        public static Maybe<T> FirstOrNothing<T>(this IEnumerable<T> source) =>
            source.Select(value => value.Just()).DefaultIfEmpty(Nothing).First();

        public static Maybe<T> FirstOrNothing<T>(this IEnumerable<T> source, Func<T, bool> predicate) =>
            source.Where(predicate).FirstOrNothing();

        public static Maybe<T> JustWhen<T>(this T value, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return predicate(value) ? value.Just() : Nothing;
        }

        public static IEnumerable<T> ToEnumerable<T>(this Maybe<T> source) =>
            source.Match(Enumerable.Return, System.Linq.Enumerable.Empty<T>);
    }
}