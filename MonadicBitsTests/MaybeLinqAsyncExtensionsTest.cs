using System;
using System.Threading.Tasks;
using MonadicBits;
using NUnit.Framework;

namespace MonadicBitsTests
{
    public static class MaybeLinqAsyncExtensionsTest
    {
        [Test]
        public static async Task Select_from_maybe_task_with_value_returns_maybe_task_with_value()
        {
            const string input = "Test";
            (await (from s in Task.FromResult(input.Just()) select s))
                .Match(s => Assert.AreEqual(input, s), Assert.Fail);
        }

        [Test]
        public static void SelectMany_with_null_collection_throws_exception() =>
            Assert.ThrowsAsync<ArgumentNullException>(() =>
                Task.FromResult("Test".Just())
                    .SelectMany((Func<string, Task<Maybe<string>>>) null, (i, c) => $"{i}{c}"));

        [Test]
        public static void SelectMany_with_null_selector_throws_exception() =>
            Assert.ThrowsAsync<ArgumentNullException>(() =>
                Task.FromResult("Test".Just())
                    .SelectMany(s => Task.FromResult(s.Just()), (Func<string, string, string>) null));

        [Test]
        public static async Task SelectMany_from_maybe_task_with_value_returns_maybe_task_with_value()
        {
            const int input = 42;
            (await (
                from s in Task.FromResult("Test".Just())
                from i in Task.FromResult(input.Just())
                select i
            )).Match(i => Assert.AreEqual(input, i), Assert.Fail);
        }
    }
}