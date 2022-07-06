using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MonadicBits.Try;

namespace MonadicBits
{
    using static Functional;

    public static partial class Functional
    {
        public static Success<T> Success<T>(this T value) => value;
        public static Failure Failure(this Exception value) => value;

        public static Try<T> Try<T>(IEnumerable<Type> types, Func<T> function)
        {
            try
            {
                return function().Success();
            }
            catch (Exception e)
            {
                if (types.Any(type => type == e.GetType()))
                {
                    return e.Failure();
                }

                throw;
            }
        }
    }

    public readonly struct Try<T>
    {
        private T Value { get; }
        private Exception Exception { get; }
        private bool IsSuccessful { get; }

        private Try(T value)
        {
            Value = value;
            Exception = default;
            IsSuccessful = true;
        }

        private Try(Exception exception)
        {
            Value = default;
            Exception = exception;
            IsSuccessful = false;
        }

        public static implicit operator Try<T>(Success<T> success) =>
            new Try<T>(success.Value);

        public static implicit operator Try<T>(Failure failure) =>
            new Try<T>(failure.Value);

        public TResult Match<TResult>([NotNull] Func<T, TResult> success, [NotNull] Func<Exception, TResult> failure)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));
            if (failure == null) throw new ArgumentNullException(nameof(failure));

            return IsSuccessful ? success(Value) : failure(Exception);
        }

        public void Match([NotNull] Action<T> success, [NotNull] Action<Exception> failure)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));
            if (failure == null) throw new ArgumentNullException(nameof(failure));

            if (IsSuccessful)
                success(Value);
            else
                failure(Exception);
        }
    }

    namespace Try
    {
        public readonly struct Success<T>
        {
            public T Value { get; }

            public Success(T value) => Value = value;

            public static implicit operator Success<T>(T value) => new Success<T>(value);
        }

        public readonly struct Failure
        {
            public Exception Value { get; }

            public Failure(Exception value) => Value = value;

            public static implicit operator Failure(Exception value) => new Failure(value);
        }
    }
}