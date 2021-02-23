namespace MonadicBits
{
    public static class MaybeExtensions
    {
        public static Maybe<T> Just<T>(this T value) => Maybe<T>.Just(value);

        public static Maybe<T> JustNotNull<T>(this T value) =>
            value == null ? Maybe<T>.Nothing() : Maybe<T>.Just(value);
    }
}
