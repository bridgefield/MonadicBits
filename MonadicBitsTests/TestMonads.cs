using MonadicBits;

namespace MonadicBitsTests
{
    public static class TestMonads
    {
        public static Maybe<T> Nothing<T>() => default;

        public static Maybe<int> JustAnInt() => 42;

        public static Either<string, string> Left(string value) =>
            value.Left();

        public static Either<string, string> Right(string value) =>
            value.Right();
    }
}