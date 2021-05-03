using MonadicBits;

namespace MonadicBitsTests
{
    public static class TestMonads
    {
        public static Maybe<T> Nothing<T>() => Functional.Nothing;

        public static Maybe<int> WithValue() => 42;
    }
}