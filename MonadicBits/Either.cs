using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MonadicBits.Either;

namespace MonadicBits
{
    using static Functional;

    public partial class Functional
    {
        public static Left<TLeft> Left<TLeft>(this TLeft value) => value;
        public static Right<TRight> Right<TRight>(this TRight value) => value;
    }

    public readonly struct Either<TLeft, TRight> : IEquatable<Either<TLeft, TRight>>
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

        /// <summary>
        /// Applies the specified binder function to the <c>Right</c> value of this <see cref="Either{L,T}"/>,
        /// returning its result. If this instance is <c>Left</c>, the existing <c>Left</c> is propagated
        /// unchanged, short-circuiting further computation.
        /// </summary>
        /// <typeparam name="TResult">The type of the resulting success value after binding.</typeparam>
        /// <param name="binder">
        /// A function that, given a <c>T</c>, returns a new <see cref="Either{L,U}"/>.  
        /// This will only be invoked if current value is <c>Right</c>.
        /// </param>
        /// <returns>
        /// A new <see cref="Either{L,U}"/>:  
        /// – If current value is <c>Left</c>, returns that <c>Left</c>.  
        /// – Otherwise, returns whatever <paramref name="binder"/> produces.
        /// </returns>
        /// <remarks>
        /// This implements the monadic “bind” (>>=) operation:
        /// <br/>
        /// - <b>Short-circuiting</b>: the first <c>Left</c> halts all further binds.
        /// <br/> 
        /// - <b>Composability</b>: you can chain many validations or computations without nested conditionals.
        /// <br/> 
        /// - <b>LINQ support</b>: with the proper <c>SelectMany</c> overload, you can write
        ///   <c>from x in … from y in … select …</c>.
        /// <br/>
        /// </remarks>
        /// <example>
        /// This clearly shows how <c>Bind</c> threads the success case forward and
        /// short-circuits on the first error.
        /// <code language="csharp">
        /// // A simple parser that can fail:
        /// Either&lt;string, int&gt;ParseInt(string s) =>
        ///     int.TryParse(s, out var n)
        ///       ? n.Right() : "Not a number".Left();
        ///
        /// // A validator that can fail:
        /// Either&lt;string, int&gt; MustBePositive(int n) =>
        ///     n > 0 ? n.Right() : "Must be positive".Left();
        ///
        /// // Chaining them with Bind:
        /// var result = ParseInt("42")
        ///     .Bind(MustBePositive);
        ///
        /// result.Match(
        ///     Left: err  => Console.WriteLine($"Error: {err}"),
        ///     Right: val => Console.WriteLine($"Value: {val}")
        /// );
        /// </code>
        /// This shows how the library integrates with existing monadic support with LINQ in C#.
        /// <code>
        /// var result = from parsedValue in ParseInt("42")
        /// from positiveValue in MustBePositive(parsedValue)
        /// select positiveValue
        /// </code>
        /// </example>
        public Either<TLeft, TResult> Bind<TResult>([NotNull] Func<TRight, Either<TLeft, TResult>> binder)
        {
            if (binder == null) throw new ArgumentNullException(nameof(binder));

            return IsRight ? binder(RightInstance) : LeftInstance.Left();
        }

        /// <summary>
        /// Applies the specified <paramref name="binder"/> function to the <c>Left</c> value of this <see cref="Either{L,T}"/>,
        /// returning its result. If this instance is <c>Right</c>, the existing <c>Right</c> is propagated unchanged,
        /// ensuring error‐side recovery or transformation without disturbing the success case.
        /// </summary>
        /// <typeparam name="TResult">The type of the resulting error after binding.</typeparam>
        /// <param name="binder">
        /// A function that, given an <c>L</c>, returns a new <see cref="Either{U,T}"/>.  
        /// This will only be invoked if current value is <c>Left</c>.
        /// </param>
        /// <returns>
        /// A new <see cref="Either{U,T}"/>:  
        /// – If current value is <c>Right</c>, returns that <c>Right</c>.  
        /// – Otherwise, returns whatever <paramref name="binder"/> produces (allowing you to recover, log, or elevate the error).
        /// </returns>
        /// <remarks>
        /// This implements an “error‐side bind”:
        /// <br/>
        /// - <b>Targeted transformation</b>: only errors are fed into the <paramref name="binder"/>, leaving successes intact.
        /// <br/> 
        /// - <b>Short‐circuiting</b>: if already <c>Right</c>, no work is done.
        /// <br/> 
        /// - <b>Composability</b>: you can chain multiple error‐handling steps.
        /// <br/>
        /// </remarks>
        /// <example>
        /// This shows how <c>BindLeft</c> can augment or recover from an error without touching the success branch.
        /// <code language="csharp">
        /// // Suppose we parse an int but want to enrich or recover the error:
        /// Either&lt;string, int&gt; ParseInt(string s) =>
        ///     int.TryParse(s, out var x)
        ///       ? x.Right()
        ///       : "Not a number".Left();
        ///
        /// // If parsing fails, try a default or wrap the message:
        /// Either&lt;string, int&gt; Recover(string err) =>
        ///     err == "Not a number"
        ///       ? $"Parsing failed: {err}".Left()
        ///       : 0.Right();  // recover with default
        ///
        /// // Use BindLeft to handle only the failure case:
        /// var result = ParseInt("abc")
        ///     .BindLeft(Recover);
        ///
        /// result.Match(
        ///    Left:  err => Console.WriteLine($"Error path: {err}"),
        ///    Right: val => Console.WriteLine($"Value path: {val}")
        /// );
        /// </code>
        /// </example>
        public Either<TResult, TRight> BindLeft<TResult>(Func<TLeft, Either<TResult, TRight>> binder)
        {
            if (binder == null) throw new ArgumentNullException(nameof(binder));

            return IsRight ? RightInstance.Right() : binder(LeftInstance);
        }

        /// <summary>
        /// Performs a pattern‐match on this <see cref="Either{L,T}"/>, invoking the corresponding handler:
        /// <paramref name="onLeft"/> if this is <c>Left</c>, or <paramref name="onRight"/> if it is <c>Right</c>.
        /// Use to fold both cases into a single resulting <typeparamref name="TResult"/>.
        /// </summary>
        /// <typeparam name="TResult">The unified return type after matching.</typeparam>
        /// <param name="onLeft">Function to handle the <c>Left</c> case, given an <c>L</c>, returning <c>R</c>.</param>
        /// <param name="onRight">Function to handle the <c>Right</c> case, given a <c>T</c>, returning <c>R</c>.</param>
        /// <returns>
        /// The result of invoking either <paramref name="onLeft"/> or <paramref name="onRight"/>,
        /// depending on this instance’s state.
        /// </returns>
        /// <remarks>
        /// Also known as “fold” or “catamorphism”:
        /// <br/>
        /// - <b>Exhaustive</b>: forces you to handle both success and error cases explicitly.
        /// <br/> 
        /// - <b>Declarative</b>: replaces `if/else` or `switch` on discriminated unions.
        /// <br/> 
        /// - <b>Terminal</b>: collapses the `Either` into a single value.
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// var result = UserFactory.Create("Bob", 20);
        ///
        /// // Fold into a message string:
        /// string message = result.Match(
        ///     onLeft: err  => $"Failed: {err}",
        ///     onRight: usr => $"Welcome, {usr.Name}!"
        /// );
        ///
        /// Console.WriteLine(message);
        /// </code>
        /// </example>
        public TResult Match<TResult>([NotNull] Func<TLeft, TResult> onLeft, [NotNull] Func<TRight, TResult> onRight)
        {
            if (onLeft == null) throw new ArgumentNullException(nameof(onLeft));
            if (onRight == null) throw new ArgumentNullException(nameof(onRight));

            return IsRight ? onRight(RightInstance) : onLeft(LeftInstance);
        }

        /// <summary>
        /// Invokes the specified action corresponding to this <see cref="Either{L,T}"/>’s state:
        /// <paramref name="onLeft"/> if this instance is <c>Left</c>, or <paramref name="onRight"/> if it is <c>Right</c>.
        /// Use when you want to perform side‐effects instead of producing a new value.
        /// </summary>
        /// <param name="onLeft">Action to perform if this is <c>Left</c>, receiving the <c>L</c> value.</param>
        /// <param name="onRight">Action to perform if this is <c>Right</c>, receiving the <c>T</c> value.</param>
        /// <remarks>
        /// Also known as “iter” or “consume”:
        /// <br/>
        /// - <b>Exhaustive handling</b>: ensures both branches are dealt with explicitly.
        /// <br/> 
        /// - <b>Side‐effects</b>: intended for logging, UI updates, or other imperative operations.
        /// <br/> 
        /// - <b>Terminal</b>: does not return a value; ends the <see cref="Either{L,T}"/> pipeline.
        /// <br/>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// var parseResult = ParseInt("xyz");
        ///
        /// // Log outcome without producing a new value:
        /// parseResult.Match(
        ///     onLeft:  err => Console.Error.WriteLine($"Parsing failed: {err}"),
        ///     onRight: val => Console.WriteLine($"Parsed value: {val}")
        /// );
        /// </code>
        /// </example>
        public void Match([NotNull] Action<TLeft> onLeft, [NotNull] Action<TRight> onRight)
        {
            if (onLeft == null) throw new ArgumentNullException(nameof(onLeft));
            if (onRight == null) throw new ArgumentNullException(nameof(onRight));

            if (IsRight)
                onRight(RightInstance);
            else
                onLeft(LeftInstance);
        }

        /// <summary>
        /// Transforms the <c>Right</c> (success) value of this <see cref="Either{L,T}"/> by applying the given <paramref name="mapping"/>,
        /// returning a new <see cref="Either{L,U}"/>. If this instance is <c>Left</c>, the existing <c>Left</c> is propagated unchanged.
        /// </summary>
        /// <typeparam name="TResult">The type of the current success value (the <c>Right</c> side).</typeparam>
        /// <param name="mapping">
        /// A function that converts a <c>T</c> into a <c>U</c>.  
        /// This will only be applied if current value is <c>Right</c>.
        /// </param>
        /// <returns>
        /// A new <see cref="Either{L,U}"/>:
        /// – If current value is <c>Left</c>, returns that <c>Left</c>.  
        /// – Otherwise, returns <c>Right(mapper(value))</c>.
        /// </returns>
        /// <remarks>
        /// Implements the functor “map” operation for <see cref="Either{L,T}"/>:
        /// - <b>Non-destructive</b>: errors pass through unchanged.
        /// - <b>Composable</b>: you can chain multiple <c>Map</c> calls to transform nested values.
        /// - <b>LINQ support</b>: this is the <c>Select</c> method that underpins LINQ’s <c>select</c> clause.
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// // Given an Either&lt;string,int&gt; result:
        /// var parseResult = ParseInt("123"); // Right(123) or Left("Not a number")
        ///
        /// // Map the int to a string with a prefix:
        /// var greeting = parseResult
        ///     .Map(n => $"You entered {n}");
        ///
        /// greeting.Match(
        ///     Left: err  => Console.WriteLine($"Error: {err}"),
        ///     Right: msg => Console.WriteLine(msg)
        /// );
        /// </code>
        /// </example>
        public Either<TLeft, TResult> Map<TResult>([NotNull] Func<TRight, TResult> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return Match<Either<TLeft, TResult>>(l => l.Left(), right => mapping(right).Right());
        }

        /// <summary>
        /// Transforms the <c>Left</c> (error) value of this <see cref="Either{L,T}"/> by applying the specified <paramref name="mapping"/>,
        /// returning a new <see cref="Either{U,T}"/>. If this instance is <c>Right</c>, the existing <c>Right</c> is propagated unchanged.
        /// </summary>
        /// <typeparam name="TResult">The type of the resulting error value after mapping.</typeparam>
        /// <param name="mapping">
        /// A function that converts the current <c>Left</c> value of type <c>L</c> into a new error value of type <c>U</c>.
        /// This will only be invoked when this <see cref="Either{L,T}"/> is in the <c>Left</c> state.
        /// </param>
        /// <returns>
        /// A new <see cref="Either{U,T}"/>:
        /// – If this is <c>Right</c>, returns <c>Right(this.RightValue)</c> unchanged.  
        /// – If this is <c>Left</c>, returns <c>Left(mapper(this.LeftValue))</c>.
        /// </returns>
        /// <remarks>
        /// This is the “error‐side map” (functor) operation:
        /// - <b>Non‐destructive</b>: success values (<c>Right</c>) pass through without change.  
        /// - <b>Composable</b>: you can chain multiple <c>MapLeft</c> calls to enrich or translate errors.  
        /// - <b>Use cases</b>: converting low‐level error codes to user‐friendly messages, wrapping exceptions, etc.
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// // Suppose we have an Either&lt;int,string&gt; from parsing:
        /// Either&lt;int,string&gt; parseResult = ParseInput("foo"); // Left(1001) or Right("parsed")
        ///
        /// // Convert the numeric error code into a descriptive message:
        /// var withMessages = parseResult.MapLeft(code => code switch {
        ///     1001 => "Invalid format",
        ///     1002 => "Value out of range",
        ///     _    => "Unknown error"
        /// });
        ///
        /// // Now handle both cases:
        /// withMessages.Match(
        ///     onLeft:  msg => Console.WriteLine($"Error: {msg}"),
        ///     onRight: val => Console.WriteLine($"Success: {val}")
        /// );
        /// </code>
        /// </example>
        public Either<TResult, TRight> MapLeft<TResult>([NotNull] Func<TLeft, TResult> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return Match<Either<TResult, TRight>>(left => mapping(left).Left(), r => r.Right());
        }

        /// <summary>
        /// Converts this <see cref="Either{L,T}"/> into a <see cref="Maybe{T}"/>,
        /// capturing only the <c>Right</c> (success) side.  
        /// If this instance is <c>Right</c>, returns <c>Just</c> of its value;  
        /// if it is <c>Left</c>, returns <c>Nothing</c>, discarding the error.
        /// </summary>
        /// <returns>
        /// A <see cref="Maybe{T}"/>:
        /// <br/>
        /// – <c>Just(value)</c> when this is <c>Right(value)</c>.
        /// <br/> 
        /// – <c>Nothing</c> when this is <c>Left(error)</c>.
        /// <br/>
        /// </returns>
        /// <remarks>
        /// This “narrowing” conversion lets you treat any failure as absence,
        /// so you can feed the result into computations expecting a <see cref="Maybe{T}"/>.
        /// It’s the natural way to drop the error information when you only care about successes.
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// // Suppose you have a parsing result:
        /// Either&lt;string,int&gt; parseResult = ParseInt("42");  // Right(42) or Left("err")
        ///
        /// // Convert to Maybe&lt;int&gt;:
        /// Maybe&lt;int&gt; maybeValue = parseResult.ToMaybe();
        ///
        /// maybeValue.Match(
        ///     Nothing: ()    => Console.WriteLine("No value"),
        ///     Just:    value => Console.WriteLine($"Parsed: {value}")
        /// );
        /// </code>
        /// </example>
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
        public readonly struct Left<T> : IEquatable<Left<T>>
        {
            public T Value { get; }

            private Left(T value) => Value = value;

            public static implicit operator Left<T>(T value) => new Left<T>(value);

            public Either<TResult, TRight>
                BindLeft<TResult, TRight>([NotNull] Func<T, Either<TResult, TRight>> mapping) =>
                Value.Left<T, TRight>().BindLeft(mapping);

            public bool Equals(Left<T> other) => EqualityComparer<T>.Default.Equals(Value, other.Value);

            public override bool Equals(object obj) => obj is Left<T> other && Equals(other);

            public override int GetHashCode() => HashCode.Combine(
                Value,
                typeof(Left<>).Name);
        }

        public readonly struct Right<T> : IEquatable<Right<T>>
        {
            public T Value { get; }

            private Right(T value) => Value = value;

            public static implicit operator Right<T>(T value) => new Right<T>(value);

            public Either<TLeft, TResult> Bind<TLeft, TResult>([NotNull] Func<T, Either<TLeft, TResult>> mapping) =>
                Value.Right<TLeft, T>().Bind(mapping);

            public bool Equals(Right<T> other) => EqualityComparer<T>.Default.Equals(Value, other.Value);

            public override bool Equals(object obj) => obj is Right<T> other && Equals(other);

            public override int GetHashCode() => HashCode.Combine(
                Value,
                typeof(Right<>).Name);
        }
    }
}