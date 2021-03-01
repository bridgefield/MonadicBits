using System;
using System.Threading.Tasks;

namespace MonadicBits
{
    public static class MaybeLinqAsyncExtensions
    {
        public static Task<Maybe<TResult>> Select<T, TResult>(this Task<Maybe<T>> maybeTask,
            Func<T, TResult> selector) =>
            maybeTask.MapAsync(selector);

        public static Task<Maybe<TResult>> SelectMany<T, TCollection, TResult>(
            this Task<Maybe<T>> maybeTask,
            Func<T, Task<Maybe<TCollection>>> collectionTask,
            Func<T, TCollection, TResult> selector)
        {
            if (collectionTask == null) throw new ArgumentNullException(nameof(collectionTask));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return maybeTask.BindAsync(m => collectionTask(m).MapAsync(result => selector(m, result)));
        }
    }
}