using System;
using System.Diagnostics.CodeAnalysis;

namespace MonadicBits
{
    public readonly struct Maybe<T>
    {
        private T Instance { get; }
        private bool IsJust { get; }

        private Maybe(T instance, bool isJust)
        {
            Instance = instance;
            IsJust = isJust;
        }

        public static Maybe<T> Just(T instance) => new Maybe<T>(instance, true);

        public static Maybe<T> Nothing() => new Maybe<T>(default, true);

        public Maybe<TResult> Bind<TResult>([NotNull] Func<T, Maybe<TResult>> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return IsJust ? mapping(Instance) : Maybe<TResult>.Nothing();
        }

        public Maybe<TResult> Map<TResult>([NotNull] Func<T, TResult> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return Match(value => Maybe<TResult>.Just(mapping(value)), Maybe<TResult>.Nothing);
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
            {
                just(Instance);
            }
            else
            {
                nothing();
            }
        }
    }
}
