using System;
using System.Threading.Tasks;
using FluentAssertions;
using MonadicBits;
using NUnit.Framework;
using static MonadicBitsTests.TestMonads;

namespace MonadicBitsTests
{
    public static class MaybeAsyncExtensionsTests
    {
        [Test]
        public static async Task MapAsync_maybe_with_value_to_async_mapping_returns_maybe_task_with_new_value()
        {
            const int input = 42;
            (await "Test".Just().MapAsync(_ => Task.FromResult(input)))
                .Should().Be(input.Just());
        }

        [Test]
        public static async Task MapAsync_empty_maybe_to_async_mapping_returns_empty_maybe_task() =>
            (await Nothing<string>().MapAsync(_ => Task.FromResult(42)))
            .Should().Be(Functional.Nothing);

        [Test]
        public static async Task MapAsync_maybe_task_with_value_to_mapping_returns_maybe_task_with_new_value()
        {
            const int input = 42;
            (await Task.FromResult("Test".Just()).MapAsync(_ => input))
                .Should().Be(input.Just());
        }

        [Test]
        public static async Task MapAsync_maybe_task_with_value_to_async_mapping_returns_maybe_task_with_new_value()
        {
            const int input = 42;
            (await Task.FromResult("Test".Just()).MapAsync(_ => Task.FromResult(input)))
                .Should().Be(input.Just());
        }

        [Test]
        public static void BindAsync_with_null_mapping_throws_exception() =>
            Assert.ThrowsAsync<ArgumentNullException>(() =>
                "Test".Just().BindAsync((Func<string, Task<Maybe<string>>>)null));

        [Test]
        public static async Task BindAsync_maybe_with_value_to_async_mapping_returns_maybe_task_with_new_value()
        {
            const int input = 42;
            (await "Test".Just().BindAsync(_ => Task.FromResult(input.Just())))
                .Should().Be(input.Just());
        }

        [Test]
        public static async Task BindAsync_empty_maybe_to_async_mapping_empty_maybe_task() =>
            (await Nothing<string>().BindAsync(_ => Task.FromResult(42.Just())))
            .Should().Be(Functional.Nothing);

        [Test]
        public static async Task BindAsync_maybe_task_with_value_to_mapping_returns_maybe_task_with_new_value()
        {
            const int input = 42;
            (await Task.FromResult("Test".Just()).BindAsync(_ => input.Just()))
                .Should().Be(input.Just());
        }

        [Test]
        public static async Task BindAsync_maybe_task_with_value_to_async_mapping_returns_maybe_task_with_new_value()
        {
            const int input = 42;
            (await Task.FromResult("Test".Just()).BindAsync(_ => Task.FromResult(input.Just())))
                .Should().Be(input.Just());
        }

        [Test]
        public static async Task Just_to_async_either_makes_right_task()
        {
            const string input = "Test";
            var result = input.Just().ToEitherAsync("Left");
            (await result).Match(Assert.Fail, right => Assert.AreEqual(input, right));
        }

        [Test]
        public static async Task Nothing_to_async_either_makes_left_task()
        {
            const string leftInput = "Left";
            var result = Nothing<string>().ToEitherAsync(leftInput);
            (await result).Match(left => Assert.AreEqual(leftInput, left), Assert.Fail);
        }

        [Test]
        public static async Task Just_task_to_async_either_makes_right_task()
        {
            const string input = "Test";
            var result = Task.FromResult(input.Just()).ToEitherAsync("Left");
            (await result).Match(Assert.Fail, right => Assert.AreEqual(input, right));
        }

        [Test]
        public static async Task Nothing_task_to_async_either_makes_left_task()
        {
            const string leftInput = "Left";
            var result = Task.FromResult(Nothing<string>()).ToEitherAsync(leftInput);
            (await result).Match(left => Assert.AreEqual(leftInput, left), Assert.Fail);
        }
    }
}
