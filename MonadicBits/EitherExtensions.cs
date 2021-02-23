namespace MonadicBits
{
    public static class EitherExtensions
    {
        public static Either<TLeft, TRight> Left<TLeft, TRight>(this TLeft left) =>
            Either<TLeft, TRight>.Left(left);

        public static Either<TLeft, TRight> Right<TLeft, TRight>(this TRight right) =>
            Either<TLeft, TRight>.Right(right);
    }
}
