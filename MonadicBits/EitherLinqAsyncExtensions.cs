using System;
using System.Threading.Tasks;

namespace MonadicBits
{
    public static class EitherLinqAsyncExtensions
    {
        public static Task<Either<TLeft, TResult>> Select<TLeft, TRight, TResult>(
            this Task<Either<TLeft, TRight>> eitherTask, Func<TRight, TResult> selector) =>
            eitherTask.MapAsync(selector);

        public static Task<Either<TLeft, TResult>> SelectMany<TLeft, TRight, TCollection, TResult>(
            this Task<Either<TLeft, TRight>> eitherTask,
            Func<TRight, Task<Either<TLeft, TCollection>>> collectionTask,
            Func<TRight, TCollection, TResult> selector)
        {
            if (collectionTask == null) throw new ArgumentNullException(nameof(collectionTask));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return eitherTask.BindAsync(m => collectionTask(m).MapAsync(result => selector(m, result)));
        }
    }
}