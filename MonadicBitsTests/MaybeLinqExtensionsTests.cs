using System;
using MonadicBits;
using NUnit.Framework;

namespace MonadicBitsTests
{
    public static class MaybeLinqExtensionsTests
    {
        [Test]
        public static void Select_from_maybe_with_value_returns_maybe_with_value()
        {
            const string input = "Test";
            (from s in input.Just() select s).Match(s => Assert.AreEqual(input, s), Assert.Fail);
        }

        [Test]
        public static void SelectMany_with_null_collection_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Just().SelectMany((Func<string, Maybe<string>>) null,
                (i, c) => $"{i}{c}"));

        [Test]
        public static void SelectMany_with_null_selector_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() =>
                "Test".Just().SelectMany(s => s.Just(), (Func<string, string, string>) null));

        [Test]
        public static void SelectMany_from_maybe_with_value_returns_maybe_with_value()
        {
            const int input = 42;
            (
                from s in "Test".Just()
                from i in input.Just()
                select i
            ).Match(i => Assert.AreEqual(input, i), Assert.Fail);
        }
    }
}