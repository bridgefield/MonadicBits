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

    public readonly struct Maybe<T> : IEquatable<Maybe<T>>
    {
        private T Instance { get; }
        private bool IsJust { get; }

        private Maybe(T instance)
        {
            Instance = instance;
            IsJust = true;
        }

        /// <summary>
        /// Applies the specified <paramref name="binder"/> function to the <c>Just</c> value of this <see cref="Maybe{T}"/>,
        /// returning its result. If this instance is <c>Nothing</c>, the existing <c>Nothing</c> is propagated.
        /// </summary>
        /// <typeparam name="TResult">The type of the <c>Just</c> value in the resulting <see cref="Maybe{U}"/>.</typeparam>
        /// <param name="binder">
        /// A function that, given a <c>T</c>, returns a new <see cref="Maybe{U}"/>.  
        /// This will only be invoked when this <see cref="Maybe{T}"/> is <c>Just</c>.
        /// </param>
        /// <returns>
        /// A <see cref="Maybe{U}"/>:
        /// <br/>
        /// – The result of <paramref name="binder"/> when this is <c>Just</c>.
        /// <br/> 
        /// – <c>Nothing</c> when this is <c>Nothing</c>.
        /// <br/>
        /// </returns>
        /// <remarks>
        /// Implements the monadic “bind” (>>=) operation:
        /// <br/>
        /// - <b>Short-circuiting</b> the first <c>Nothing</c> halts all further binds.
        /// <br/> 
        /// - <b>Composability</b> chain validations or lookups without nested conditionals.
        /// <br/> 
        /// - <b>LINQ support</b> enables <c>from…select</c> syntax via <c>SelectMany</c>.
        /// <br/>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// Maybe&lt;string&gt; ReadEnvVar(string name) =>Environment.GetEnvironmentVariable(name).JustNotNull();
        /// Maybe&lt;int&gt; Parse(string s) => int.TryParse(s, out var x) ? x.Just() : default;
        ///
        /// var result = "PORT".Just()
        ///     .Bind(ReadEnvVar)    // Maybe&lt;string&gt;
        ///     .Bind(Parse);        // Maybe&lt;int&gt;
        ///
        /// result.Match(
        ///   just:    port  => Console.WriteLine($"Listening on port {port}"),
        ///   nothing: ()    => Console.WriteLine("Could not determine port")
        /// );
        /// </code>
        /// </example>
        public Maybe<TResult> Bind<TResult>([NotNull] Func<T, Maybe<TResult>> binder) =>
            Match(binder, () => Nothing);

        /// <summary>
        /// Transforms the value inside this <see cref="Maybe{T}"/> by applying the specified <paramref name="mapping"/>,
        /// returning a new <see cref="Maybe{U}"/>.  
        /// If this instance is <c>Nothing</c>, the mapping function is not invoked and <c>Nothing</c> is returned.
        /// </summary>
        /// <typeparam name="T">The type of the current <c>Just</c> value.</typeparam>
        /// <typeparam name="TResult">The type of the resulting <c>Just</c> value after mapping.</typeparam>
        /// <param name="mapping">
        /// A function that, given a <c>T</c>, produces a <c>U</c>.  
        /// This will only be applied when this <see cref="Maybe{T}"/> is <c>Just</c>.
        /// </param>
        /// <returns>
        /// A <see cref="Maybe{U}"/>:
        /// <br/>
        /// – <c>Just(mapper(value))</c> when this is <c>Just(value)</c>.
        /// <br/> 
        /// – <c>Nothing</c> when this is <c>Nothing</c>.
        /// <br/>
        /// </returns>
        /// <remarks>
        /// Implements the functor “map” operation:
        /// <br/>
        /// - <b>Non-destructive</b>: nothing passes through unchanged.
        /// <br/> 
        /// - <b>Composable</b> chain multiple <c>Map</c> calls to transform nested values.
        /// <br/> 
        /// - <b>LINQ support</b> underpins the LINQ <c>select</c> clause when used with <c>Bind</c>.
        /// <br/>
        /// </remarks>
        /// <example>
        /// Usage of map on just and nothing values
        /// <code language="csharp">
        /// Maybe&lt;int&gt; maybeNumber = 5.Just();
        /// Maybe&lt;string&gt; maybeText = maybeNumber.Map(n => $"Number is {n}");
        /// // maybeText is Just("Number is 5")
        ///
        /// Maybe&lt;int&gt; nothing = new Maybe&lt;int&gt;();
        /// var stillNone = nothing.Map(n => n * 2);
        /// // stillNothing
        /// </code>
        /// alternative instantiations of Nothing
        /// <code language="csharp">
        /// // default struct initialization
        /// Maybe&lt;int&gt; nothing = default;
        /// // static default value
        /// using static Functional; //at start of namespace to import functional helper methods
        /// ...
        /// Maybe&lt;int&gt; nothing = Nothing;
        /// // Nothing temp struct with implicit conversion
        /// Maybe&lt;int&gt; nothing = new Nothing();
        /// </code>
        /// </example>
        public Maybe<TResult> Map<TResult>([NotNull] Func<T, TResult> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return Match(value => mapping(value).Just(), () => Nothing);
        }

        /// <summary>
        /// Folds this <see cref="Maybe{T}"/> into a single <typeparamref name="TResult"/> by invoking
        /// <paramref name="onNothing"/> if it is <c>Nothing</c>, or <paramref name="onJust"/> if it is <c>Just</c>.
        /// </summary>
        /// <typeparam name="TResult">The result type after folding.</typeparam>
        /// <param name="onNothing">Function to invoke when this is <c>Nothing</c>, returning <c>R</c>.</param>
        /// <param name="onJust">Function to invoke when this is <c>Just</c>, given the <c>T</c> value, returning <c>R</c>.</param>
        /// <returns>
        /// The value produced by <paramref name="onNothing"/> or <paramref name="onJust"/>,
        /// depending on this instance’s state.
        /// </returns>
        /// <remarks>
        /// Also known as “fold” or “catamorphism”:
        /// <br/>
        /// - <b>Exhaustive</b>: forces you to handle both absence and presence.
        /// <br/> 
        /// - <b>Terminal</b>: collapses the <see cref="Maybe{T}"/> into a single value.
        /// <br/> 
        /// - <b>Declarative</b>: replaces null-checks or if/else ladders.
        /// <br/>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// Maybe&lt;int&gt; maybeCount = GetItemCount();
        /// string message = maybeCount.Match(
        ///     onJust:    count => $"Found {count} items",
        ///     onNothing: ()    => "No items found"
        /// );
        /// Console.WriteLine(message);
        /// </code>
        /// </example>
        public TResult Match<TResult>([NotNull] Func<T, TResult> onJust, [NotNull] Func<TResult> onNothing)
        {
            if (onJust == null) throw new ArgumentNullException(nameof(onJust));
            if (onNothing == null) throw new ArgumentNullException(nameof(onNothing));

            return IsJust ? onJust(Instance) : onNothing();
        }

        /// <summary>
        /// Executes the given <paramref name="onNothing"/> or <paramref name="onJust"/> action based on whether
        /// this <see cref="Maybe{T}"/> is <c>Nothing</c> or <c>Just</c>, respectively.
        /// </summary>
        /// <param name="onNothing">Action to execute when this is <c>Nothing</c>.</param>
        /// <param name="onJust">Action to execute when this is <c>Just</c>, receiving the <c>T</c> value.</param>
        /// <remarks>
        /// Use when you want to perform side-effects rather than produce a new value.
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// Maybe&lt;string&gt; maybeName = GetUserName();
        /// maybeName.Match(
        ///   onJust:    name  => Console.WriteLine($"Hello, {name}!"),
        ///   onNothing: ()    => Console.WriteLine("Name not provided")
        /// );
        /// </code>
        /// </example>
        public void Match([NotNull] Action<T> onJust, [NotNull] Action onNothing)
        {
            if (onJust == null) throw new ArgumentNullException(nameof(onJust));
            if (onNothing == null) throw new ArgumentNullException(nameof(onNothing));

            if (IsJust)
                onJust(Instance);
            else
                onNothing();
        }

        /// <summary>
        /// Converts this <see cref="Maybe{T}"/> into an <see cref="Either{L,T}"/>,
        /// using the specified <paramref name="leftValue"/> when this is <c>Nothing</c>.
        /// </summary>
        /// <typeparam name="T">The type of the <c>Just</c> value.</typeparam>
        /// <typeparam name="TLeft">The type of the <c>Left</c> error when converting <c>Nothing</c>.</typeparam>
        /// <param name="leftValue">The value to use for <c>Left</c> if this is <c>Nothing</c>.</param>
        /// <returns>
        /// An <see cref="Either{L,T}"/>:
        /// <br/>
        /// – <c>Right(value)</c> when this is <c>Just(value)</c>.
        /// <br/> 
        /// – <c>Left(leftValue)</c> when this is <c>Nothing</c>.
        /// <br/>
        /// </returns>
        /// <remarks>
        /// Lets you drop absence semantics into workflows that require an <see cref="Either{L,T}"/>,
        /// supplying a default error for the missing case.
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// Maybe&lt;int&gt; maybeId = LookupUserId("alice");
        /// Either&lt;string,int&gt; result = maybeId.ToEither("User not found");
        ///
        /// result.Match(
        ///   onLeft:  err => Console.WriteLine($"Error: {err}"),
        ///   onRight: id  => Console.WriteLine($"User ID: {id}")
        /// );
        /// </code>
        /// </example>
        public Either<TLeft, T> ToEither<TLeft>([NotNull] TLeft leftValue) =>
            Match<Either<TLeft, T>>(i => i.Right(), () => leftValue.Left());

        public static implicit operator Maybe<T>(Nothing _) => new Maybe<T>();

        public static implicit operator Maybe<T>(T value) =>
            value == null ? Nothing : new Maybe<T>(value);

        public override string ToString() => IsJust ? $"Just: {{{Instance}}}" : nameof(Nothing);

        public override int GetHashCode() => IsJust ? Instance.GetHashCode() : 0;

        public override bool Equals(object obj) =>
            obj switch
            {
                Nothing _ => !IsJust,
                Maybe<T> other => Equals(other),
                T otherValue when IsJust => Equals(Instance, otherValue),
                _ => false
            };

        public bool Equals(Maybe<T> other) =>
            other.IsJust == IsJust && Equals(other.Instance, Instance);
    }

    namespace Maybe
    {
        public readonly struct Nothing : IEquatable<Nothing>
        {
            public override int GetHashCode() => 0;

            public override bool Equals(object obj) =>
                obj is Nothing || obj != null && obj.Equals(this);

            public bool Equals(Nothing other) => true;
        }
    }
}