using System;
using MonadicBits;
using NUnit.Framework;

namespace MonadicBitsTests
{
    public static class EitherLinqExtensionsTests
    {
        [Test]
        public static void Select_from_either_with_right_value_returns_either_with_right_value()
        {
            const string input = "Test";
            (from s in input.Right<string, string>() select s).Match(Assert.Fail, s => Assert.AreEqual(input, s));
        }

        [Test]
        public static void SelectMany_with_null_collection_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Right<string, string>().SelectMany(
                (Func<string, Either<string, string>>) null, (i, c) => $"{i}{c}"));

        [Test]
        public static void SelectMany_with_null_selector_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() =>
                "Test".Right<string, string>()
                    .SelectMany(s => s.Right<string, string>(), (Func<string, string, string>) null));

        [Test]
        public static void SelectMany_from_either_with_right_value_returns_either_with_right_value()
        {
            const int input = 42;
            (
                from s in "Test".Right<string, string>()
                from i in input.Right<string, int>()
                select i
            ).Match(Assert.Fail, i => Assert.AreEqual(input, i));
        }
    }
}