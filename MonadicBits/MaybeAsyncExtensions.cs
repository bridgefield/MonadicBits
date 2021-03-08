﻿using System;
using System.Threading.Tasks;

namespace MonadicBits
{
    public static class MaybeAsyncExtensions
    {
        public static Task<Maybe<TResult>> MapAsync<T, TResult>(this Maybe<T> maybe,
            Func<T, Task<TResult>> mapping, bool continueOnCapturedContext = false) =>
            maybe.Map(mapping).Match(
                async task => (await task.ConfigureAwait(continueOnCapturedContext)).Just(),
                () => Task.FromResult(Maybe<TResult>.Nothing()));

        public static async Task<Maybe<TResult>> MapAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
            Func<T, TResult> mapping, bool continueOnCapturedContext = false) =>
            (await maybeTask.ConfigureAwait(continueOnCapturedContext)).Map(mapping);

        public static async Task<Maybe<TResult>> MapAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
            Func<T, Task<TResult>> mapping, bool continueOnCapturedContext = false) =>
            await (await maybeTask.ConfigureAwait(continueOnCapturedContext))
                .MapAsync(mapping, continueOnCapturedContext);

        public static Task<Maybe<TResult>> BindAsync<T, TResult>(this Maybe<T> maybe,
            Func<T, Task<Maybe<TResult>>> mapping, bool continueOnCapturedContext = false)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return maybe.Match(
                async value => await mapping(value).ConfigureAwait(continueOnCapturedContext),
                () => Task.FromResult(Maybe<TResult>.Nothing()));
        }

        public static async Task<Maybe<TResult>> BindAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
            Func<T, Maybe<TResult>> mapping, bool continueOnCapturedContext = false) =>
            (await maybeTask.ConfigureAwait(continueOnCapturedContext)).Bind(mapping);

        public static async Task<Maybe<TResult>> BindAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
            Func<T, Task<Maybe<TResult>>> mapping, bool continueOnCapturedContext = false) =>
            await (await maybeTask.ConfigureAwait(continueOnCapturedContext)).BindAsync(mapping,
                continueOnCapturedContext);
    }
}