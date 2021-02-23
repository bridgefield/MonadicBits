using System;
using System.Diagnostics.CodeAnalysis;

namespace MonadicBits
{
    public readonly struct Either<TLeft, TRight>
    {
        private TLeft LeftInstance { get; }
        private TRight RightInstance { get; }
        private bool IsRight { get; }

        private Either(TLeft leftInstance)
        {
            LeftInstance = leftInstance;
            RightInstance = default;
            IsRight = false;
        }

        private Either(TRight rightInstance)
        {
            LeftInstance = default;
            RightInstance = rightInstance;
            IsRight = true;
        }

        public static Either<TLeft, TRight> Left(TLeft instance) => new Either<TLeft, TRight>(instance);

        public static Either<TLeft, TRight> Right(TRight instance) => new Either<TLeft, TRight>(instance);

        public Either<TLeft, TResult> Bind<TResult>([NotNull] Func<TRight, Either<TLeft, TResult>> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return IsRight ? mapping(RightInstance) : Either<TLeft, TResult>.Left(LeftInstance);
        }

        public Either<TResult, TRight> BindLeft<TResult>(Func<TLeft, Either<TResult, TRight>> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return IsRight ? Either<TResult, TRight>.Right(RightInstance) : mapping(LeftInstance);
        }

        public TResult Match<TResult>([NotNull] Func<TLeft, TResult> left, [NotNull] Func<TRight, TResult> right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            return IsRight ? right(RightInstance) : left(LeftInstance);
        }

        public void Match([NotNull] Action<TLeft> left, [NotNull] Action<TRight> right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            if (IsRight)
                right(RightInstance);
            else
                left(LeftInstance);
        }

        public Either<TLeft, TResult> Map<TResult>([NotNull] Func<TRight, TResult> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return Match(Either<TLeft, TResult>.Left, right => Either<TLeft, TResult>.Right(mapping(right)));
        }

        public Either<TResult, TRight> MapLeft<TResult>([NotNull] Func<TLeft, TResult> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return Match(left => Either<TResult, TRight>.Left(mapping(left)), Either<TResult, TRight>.Right);
        }

        public Maybe<TRight> ToMaybe() => Match(_ => Maybe<TRight>.Nothing(), Maybe<TRight>.Just);
    }
}
