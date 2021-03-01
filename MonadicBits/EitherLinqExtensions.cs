using System;

namespace MonadicBits
{
    public static class EitherLinqExtensions
    {
        public static Either<TLeft, TResult> Select<TLeft, TRight, TResult>(this Either<TLeft, TRight> either,
            Func<TRight, TResult> selector) =>
            either.Map(selector);

        public static Either<TLeft, TResult> SelectMany<TLeft, TRight, TCollection, TResult>(
            this Either<TLeft, TRight> either,
            Func<TRight, Either<TLeft, TCollection>> collection,
            Func<TRight, TCollection, TResult> selector)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return either.Bind(m => collection(m).Map(result => selector(m, result)));
        }
    }
}