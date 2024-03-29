﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using MonadicBits.Either;

namespace MonadicBits
{
    using static Functional;

    public partial class Functional
    {
        public static Left<TLeft> Left<TLeft>(this TLeft value) => value;
        public static Right<TRight> Right<TRight>(this TRight value) => value;
    }

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

        public Either<TLeft, TResult> Bind<TResult>([NotNull] Func<TRight, Either<TLeft, TResult>> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return IsRight ? mapping(RightInstance) : LeftInstance.Left();
        }

        public Either<TResult, TRight> BindLeft<TResult>(Func<TLeft, Either<TResult, TRight>> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return IsRight ? RightInstance.Right() : mapping(LeftInstance);
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

            return Match<Either<TLeft, TResult>>(l => l.Left(), right => mapping(right).Right());
        }

        public Either<TResult, TRight> MapLeft<TResult>([NotNull] Func<TLeft, TResult> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return Match<Either<TResult, TRight>>(left => mapping(left).Left(), r => r.Right());
        }

        public Maybe<TRight> ToMaybe() => Match(_ => Nothing, right => right.Just());

        public static implicit operator Either<TLeft, TRight>(Left<TLeft> left) =>
            new Either<TLeft, TRight>(left.Value);

        public static implicit operator Either<TLeft, TRight>(Right<TRight> right) =>
            new Either<TLeft, TRight>(right.Value);

        public override string ToString() =>
            IsRight ? $"Right: {{{RightInstance}}}" : $"Left: {{{LeftInstance}}}";

        public bool Equals(Either<TLeft, TRight> other) =>
            IsRight == other.IsRight &&
            (IsRight ? Equals(other.RightInstance, RightInstance) : Equals(other.LeftInstance, LeftInstance));

        public override bool Equals(object obj) =>
            obj switch
            {
                Either<TLeft, TRight> other => Equals(other),
                Right<TRight> right when IsRight => Equals(right.Value, RightInstance),
                Left<TLeft> left when !IsRight => Equals(left.Value, LeftInstance),
                _ => false
            };

        public override int GetHashCode() =>
            IsRight ? RightInstance.Right().GetHashCode() : LeftInstance.Left().GetHashCode();
    }

    namespace Either
    {
        public readonly struct Left<T>
        {
            public T Value { get; }

            public Left(T value) => Value = value;

            public static implicit operator Left<T>(T value) => new Left<T>(value);

            public Either<TResult, TRight>
                BindLeft<TResult, TRight>([NotNull] Func<T, Either<TResult, TRight>> mapping) =>
                Value.Left<T, TRight>().BindLeft(mapping);
        }

        public readonly struct Right<T>
        {
            public T Value { get; }

            public Right(T value) => Value = value;

            public static implicit operator Right<T>(T value) => new Right<T>(value);

            public Either<TLeft, TResult> Bind<TLeft, TResult>([NotNull] Func<T, Either<TLeft, TResult>> mapping) =>
                Value.Right<TLeft, T>().Bind(mapping);
        }
    }
}