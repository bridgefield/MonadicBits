using System;
using System.Threading.Tasks;
using MonadicBits;
using NUnit.Framework;

namespace MonadicBitsTests
{
    public static class EitherAsyncExtensionsTests
    {
        [Test]
        public static async Task MapAsync_on_right_returns_task_with_new_right()
        {
            const int input = 42;
            (await "Test".Right<string, string>().MapAsync(_ => Task.FromResult(input)))
                .Match(Assert.Fail, i => Assert.AreEqual(input, i));
        }

        [Test]
        public static async Task MapLeftAsync_on_left_returns_task_with_new_left()
        {
            const int input = 42;
            (await "Test".Left<string, string>().MapLeftAsync(_ => Task.FromResult(input)))
                .Match(i => Assert.AreEqual(input, i), Assert.Fail);
        }

        [Test]
        public static async Task MapAsync_right_task_returns_task_with_new_right()
        {
            const int input = 42;
            (await Task.FromResult("Test".Right<string, string>()).MapAsync(_ => input))
                .Match(Assert.Fail, i => Assert.AreEqual(input, i));
        }

        [Test]
        public static async Task MapLeftAsync_left_task_returns_task_with_new_left()
        {
            const int input = 42;
            (await Task.FromResult("Test".Left<string, string>()).MapLeftAsync(_ => input))
                .Match(i => Assert.AreEqual(input, i), Assert.Fail);
        }

        [Test]
        public static async Task
            MapAsync_either_task_with_right_value_to_async_mapping_returns_either_task_with_new_right_value()
        {
            const int input = 42;
            (await Task.FromResult("Test".Right<string, string>()).MapAsync(_ => Task.FromResult(input)))
                .Match(Assert.Fail, i => Assert.AreEqual(input, i));
        }

        [Test]
        public static async Task
            MapLeftAsync_either_task_with_left_value_to_async_mapping_returns_either_task_with_new_left_value()
        {
            const int input = 42;
            (await Task.FromResult("Test".Left<string, string>()).MapLeftAsync(_ => Task.FromResult(input)))
                .Match(i => Assert.AreEqual(input, i), Assert.Fail);
        }

        [Test]
        public static async Task MapAsync_either_with_left_value_to_async_mapping_returns_either_with_left_value()
        {
            const string input = "Test";
            (await input.Left<string, string>().MapAsync(_ => Task.FromResult(42)))
                .Match(s => Assert.AreEqual(input, s), _ => Assert.Fail());
        }

        [Test]
        public static async Task MapLeftAsync_either_with_right_value_to_async_mapping_returns_either_with_right_value()
        {
            const string input = "Test";
            (await input.Right<string, string>().MapLeftAsync(_ => Task.FromResult(42)))
                .Match(_ => Assert.Fail(), s => Assert.AreEqual(input, s));
        }

        [Test]
        public static void BindAsync_with_null_mapping_throws_exception() =>
            Assert.ThrowsAsync<ArgumentNullException>(() =>
                "Test".Right<string, string>().BindAsync((Func<string, Task<Either<string, string>>>) null));

        [Test]
        public static void BindLeftAsync_with_null_mapping_throws_exception() =>
            Assert.ThrowsAsync<ArgumentNullException>(() =>
                "Test".Left<string, string>().BindLeftAsync((Func<string, Task<Either<string, string>>>) null));

        [Test]
        public static async Task
            BindAsync_either_with_right_value_to_async_mapping_returns_eiter_task_with_new_right_value()
        {
            const int input = 42;
            (await "Test".Right<string, string>().BindAsync(_ => Task.FromResult(input.Right<string, int>())))
                .Match(Assert.Fail, i => Assert.AreEqual(input, i));
        }

        [Test]
        public static async Task
            BindLeftAsync_either_with_left_value_to_async_mapping_returns_either_task_with_new_left_value()
        {
            const int input = 42;
            (await "Test".Left<string, string>().BindLeftAsync(_ => Task.FromResult(input.Left<int, string>())))
                .Match(i => Assert.AreEqual(input, i), Assert.Fail);
        }

        [Test]
        public static async Task BindAsync_either_with_left_value_async_mapping_either_task_with_left_value()
        {
            const string input = "Test";
            (await input.Left<string, string>().BindAsync(_ => Task.FromResult(42.Right<string, int>())))
                .Match(s => Assert.AreEqual(input, s), _ => Assert.Fail());
        }

        [Test]
        public static async Task BindLeftAsync_either_with_right_value_async_mapping_either_task_with_right_value()
        {
            const string input = "Test";
            (await input.Right<string, string>().BindLeftAsync(_ => Task.FromResult(42.Left<int, string>())))
                .Match(_ => Assert.Fail(), s => Assert.AreEqual(input, s));
        }

        [Test]
        public static async Task
            BindAsync_either_task_with_right_value_to_mapping_returns_either_task_with_new_right_value()
        {
            const int input = 42;
            (await Task.FromResult("Test".Right<string, string>()).BindAsync(_ => input.Right<string, int>()))
                .Match(_ => Assert.Fail(), i => Assert.AreEqual(input, i));
        }

        [Test]
        public static async Task
            BindLeftAsync_either_task_with_left_value_to_mapping_returns_either_task_with_new_left_value()
        {
            const int input = 42;
            (await Task.FromResult("Test".Left<string, string>()).BindLeftAsync(_ => input.Left<int, string>()))
                .Match(i => Assert.AreEqual(input, i), _ => Assert.Fail());
        }

        [Test]
        public static async Task
            BindAsync_either_task_with_right_value_to_async_mapping_returns_either_task_with_new_right_value()
        {
            const int input = 42;
            (await Task.FromResult("Test".Right<string, string>())
                    .BindAsync(_ => Task.FromResult(input.Right<string, int>())))
                .Match(_ => Assert.Fail(), i => Assert.AreEqual(input, i));
        }

        [Test]
        public static async Task
            BindLeftAsync_either_task_with_left_value_to_async_mapping_returns_either_task_with_new_left_value()
        {
            const int input = 42;
            (await Task.FromResult("Test".Left<string, string>())
                    .BindLeftAsync(_ => Task.FromResult(input.Left<int, string>())))
                .Match(i => Assert.AreEqual(input, i), _ => Assert.Fail());
        }
    }
}