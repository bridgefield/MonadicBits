using System;
using System.Diagnostics.CodeAnalysis;
using MonadicBits.Maybe;

namespace MonadicBits
{
    using static Functional;

    public static partial class Functional
    {
        public static Nothing Nothing = default;
    }

    public readonly struct Maybe<T>
    {
        private T Instance { get; }
        private bool IsJust { get; }

        private Maybe(T instance)
        {
            Instance = instance;
            IsJust = true;
        }

        public Maybe<TResult> Bind<TResult>([NotNull] Func<T, Maybe<TResult>> mapping) =>
            Match(mapping, () => Nothing);

        public Maybe<TResult> Map<TResult>([NotNull] Func<T, TResult> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return Match(value => mapping(value).Just(), () => Nothing);
        }

        public TResult Match<TResult>([NotNull] Func<T, TResult> just, [NotNull] Func<TResult> nothing)
        {
            if (just == null) throw new ArgumentNullException(nameof(just));
            if (nothing == null) throw new ArgumentNullException(nameof(nothing));

            return IsJust ? just(Instance) : nothing();
        }

        public void Match([NotNull] Action<T> just, [NotNull] Action nothing)
        {
            if (just == null) throw new ArgumentNullException(nameof(just));
            if (nothing == null) throw new ArgumentNullException(nameof(nothing));

            if (IsJust)
                just(Instance);
            else
                nothing();
        }

        public Either<TLeft, T> ToEither<TLeft>([NotNull] TLeft left) =>
            Match<Either<TLeft, T>>(i => i.Right(), () => left.Left());

        public static implicit operator Maybe<T>(Nothing _) => new Maybe<T>();

        public static implicit operator Maybe<T>(T value) =>
            value == null ? Nothing : new Maybe<T>(value);
    }

    namespace Maybe
    {
        public readonly struct Nothing
        {
        }
    }
}