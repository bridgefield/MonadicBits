using System;
using System.Threading.Tasks;

namespace MonadicBits
{
    public static class EitherAsyncExtensions
    {
        public static Task<Either<TLeft, TResult>> MapAsync<TLeft, TRight, TResult>(
            this Either<TLeft, TRight> either, Func<TRight, Task<TResult>> mapping,
            bool continueOnCapturedContext = false) =>
            either.Map(mapping).Match(
                left => Task.FromResult(left.Left<TLeft, TResult>()),
                async task => (await task.ConfigureAwait(continueOnCapturedContext)).Right<TLeft, TResult>());

        public static Task<Either<TResult, TRight>> MapLeftAsync<TLeft, TRight, TResult>(
            this Either<TLeft, TRight> either, Func<TLeft, Task<TResult>> mapping,
            bool continueOnCapturedContext = false) =>
            either.MapLeft(mapping).Match(
                async task => (await task.ConfigureAwait(continueOnCapturedContext)).Left<TResult, TRight>(),
                right => Task.FromResult(right.Right<TResult, TRight>()));

        public static async Task<Either<TLeft, TResult>> MapAsync<TLeft, TRight, TResult>(
            this Task<Either<TLeft, TRight>> eitherTask, Func<TRight, TResult> mapping,
            bool continueOnCapturedContext = false) =>
            (await eitherTask.ConfigureAwait(continueOnCapturedContext)).Map(mapping);

        public static async Task<Either<TResult, TRight>> MapLeftAsync<TLeft, TRight, TResult>(
            this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, TResult> mapping,
            bool continueOnCapturedContext = false) =>
            (await eitherTask.ConfigureAwait(continueOnCapturedContext)).MapLeft(mapping);

        public static async Task<Either<TLeft, TResult>> MapAsync<TLeft, TRight, TResult>(
            this Task<Either<TLeft, TRight>> eitherTask, Func<TRight, Task<TResult>> mapping,
            bool continueOnCapturedContext = false) =>
            await (await eitherTask.ConfigureAwait(continueOnCapturedContext))
                .MapAsync(mapping, continueOnCapturedContext);

        public static async Task<Either<TResult, TRight>> MapLeftAsync<TLeft, TRight, TResult>(
            this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, Task<TResult>> mapping,
            bool continueOnCapturedContext = false) =>
            await (await eitherTask.ConfigureAwait(continueOnCapturedContext))
                .MapLeftAsync(mapping, continueOnCapturedContext);

        public static Task<Either<TLeft, TResult>> BindAsync<TLeft, TRight, TResult>(
            this Either<TLeft, TRight> either, Func<TRight, Task<Either<TLeft, TResult>>> mapping,
            bool continueOnCapturedContext = false)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return either.Match(
                left => Task.FromResult(left.Left<TLeft, TResult>()),
                async right => await mapping(right).ConfigureAwait(continueOnCapturedContext));
        }

        public static Task<Either<TResult, TRight>> BindLeftAsync<TLeft, TRight, TResult>(
            this Either<TLeft, TRight> either, Func<TLeft, Task<Either<TResult, TRight>>> mapping,
            bool continueOnCapturedContext = false)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return either.Match(
                async left => await mapping(left).ConfigureAwait(continueOnCapturedContext),
                right => Task.FromResult(right.Right<TResult, TRight>()));
        }

        public static async Task<Either<TLeft, TResult>> BindAsync<TLeft, TRight, TResult>(
            this Task<Either<TLeft, TRight>> eitherTask, Func<TRight, Either<TLeft, TResult>> mapping,
            bool continueOnCapturedContext = false) =>
            (await eitherTask.ConfigureAwait(continueOnCapturedContext)).Bind(mapping);

        public static async Task<Either<TResult, TRight>> BindLeftAsync<TLeft, TRight, TResult>(
            this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, Either<TResult, TRight>> mapping,
            bool continueOnCapturedContext = false) =>
            (await eitherTask.ConfigureAwait(continueOnCapturedContext)).BindLeft(mapping);

        public static async Task<Either<TLeft, TResult>> BindAsync<TLeft, TRight, TResult>(
            this Task<Either<TLeft, TRight>> eitherTask, Func<TRight, Task<Either<TLeft, TResult>>> mapping,
            bool continueOnCapturedContext = false) =>
            await (await eitherTask.ConfigureAwait(continueOnCapturedContext)).BindAsync(mapping,
                continueOnCapturedContext);

        public static async Task<Either<TResult, TRight>> BindLeftAsync<TLeft, TRight, TResult>(
            this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, Task<Either<TResult, TRight>>> mapping,
            bool continueOnCapturedContext = false) =>
            await (await eitherTask.ConfigureAwait(continueOnCapturedContext)).BindLeftAsync(mapping,
                continueOnCapturedContext);
    }
}