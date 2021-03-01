using System;
using System.Threading.Tasks;
using MonadicBits;
using NUnit.Framework;

namespace MonadicBitsTests
{
    public static class MaybeAsyncExtensionsTests
    {
        [Test]
        public static async Task MapAsync_maybe_with_value_to_async_mapping_returns_maybe_task_with_new_value()
        {
            const int input = 42;
            (await "Test".Just().MapAsync(_ => Task.FromResult(input)))
                .Match(i => Assert.AreEqual(input, i), Assert.Fail);
        }

        [Test]
        public static async Task MapAsync_empty_maybe_to_async_mapping_returns_empty_maybe_task() =>
            (await Maybe<string>.Nothing().MapAsync(_ => Task.FromResult(42)))
            .Match(_ => Assert.Fail(), Assert.Pass);

        [Test]
        public static async Task MapAsync_maybe_task_with_value_to_mapping_returns_maybe_task_with_new_value()
        {
            const int input = 42;
            (await Task.FromResult("Test".Just()).MapAsync(_ => input))
                .Match(i => Assert.AreEqual(input, i), Assert.Fail);
        }

        [Test]
        public static async Task MapAsync_maybe_task_with_value_to_async_mapping_returns_maybe_task_with_new_value()
        {
            const int input = 42;
            (await Task.FromResult("Test".Just()).MapAsync(_ => Task.FromResult(input)))
                .Match(i => Assert.AreEqual(input, i), Assert.Fail);
        }

        [Test]
        public static void BindAsync_with_null_mapping_throws_exception() =>
            Assert.ThrowsAsync<ArgumentNullException>(() =>
                "Test".Just().BindAsync((Func<string, Task<Maybe<string>>>) null));

        [Test]
        public static async Task BindAsync_maybe_with_value_to_async_mapping_returns_maybe_task_with_new_value()
        {
            const int input = 42;
            (await "Test".Just().BindAsync(_ => Task.FromResult(input.Just())))
                .Match(i => Assert.AreEqual(input, i), Assert.Fail);
        }

        [Test]
        public static async Task BindAsync_empty_maybe_to_async_mapping_empty_maybe_task() =>
            (await Maybe<string>.Nothing().BindAsync(_ => Task.FromResult(42.Just())))
            .Match(_ => Assert.Fail(), Assert.Pass);

        [Test]
        public static async Task BindAsync_maybe_task_with_value_to_mapping_returns_maybe_task_with_new_value()
        {
            const int input = 42;
            (await Task.FromResult("Test".Just()).BindAsync(_ => input.Just()))
                .Match(i => Assert.AreEqual(input, i), Assert.Fail);
        }

        [Test]
        public static async Task BindAsync_maybe_task_with_value_to_async_mapping_returns_maybe_task_with_new_value()
        {
            const int input = 42;
            (await Task.FromResult("Test".Just()).BindAsync(_ => Task.FromResult(input.Just())))
                .Match(i => Assert.AreEqual(input, i), Assert.Fail);
        }
    }
}