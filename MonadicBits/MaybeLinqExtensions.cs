using System;

namespace MonadicBits
{
    public static class MaybeLinqExtensions
    {
        public static Maybe<TResult> Select<T, TResult>(this Maybe<T> maybe, Func<T, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return maybe.Map(selector);
        }

        public static Maybe<TResult> SelectMany<T, TCollection, TResult>(
            this Maybe<T> maybe,
            Func<T, Maybe<TCollection>> collection,
            Func<T, TCollection, TResult> selector)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return maybe.Bind(m => collection(m).Map(result => selector(m, result)));
        }
    }
}