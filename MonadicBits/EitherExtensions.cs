namespace MonadicBits
{
    public static class EitherExtensions
    {
        public static Either<TLeft, TRight> Left<TLeft, TRight>(this TLeft value) =>
            value.Left();

        public static Either<TLeft, TRight> Right<TLeft, TRight>(this TRight value) =>
            value.Right();
    }
}