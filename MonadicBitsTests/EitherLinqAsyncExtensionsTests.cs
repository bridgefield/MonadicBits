using System;
using System.Threading.Tasks;
using MonadicBits;
using NUnit.Framework;

namespace MonadicBitsTests
{
    public static class EitherLinqAsyncExtensionsTests
    {
        [Test]
        public static async Task Select_from_either_task_with_right_value_returns_either_task_with_right_value()
        {
            const string input = "Test";
            (await (from s in Task.FromResult(input.Right<string, string>()) select s))
                .Match(Assert.Fail, s => Assert.AreEqual(input, s));
        }

        [Test]
        public static void SelectMany_with_null_collection_throws_exception() =>
            Assert.ThrowsAsync<ArgumentNullException>(() => Task.FromResult("Test".Right<string, string>())
                .SelectMany((Func<string, Task<Either<string, string>>>) null, (i, c) => $"{i}{c}"));

        [Test]
        public static void SelectMany_with_null_selector_throws_exception() =>
            Assert.ThrowsAsync<ArgumentNullException>(() =>
                Task.FromResult("Test".Right<string, string>())
                    .SelectMany(s => Task.FromResult(s.Right<string, string>()), (Func<string, string, string>) null));

        [Test]
        public static async Task SelectMany_from_either_task_with_right_value_returns_either_task_with_right_value()
        {
            const int input = 42;
            (await (
                from s in Task.FromResult("Test".Right<string, string>())
                from i in Task.FromResult(input.Right<string, int>())
                select i
            )).Match(Assert.Fail, i => Assert.AreEqual(input, i));
        }
    }
}