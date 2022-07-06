using System;
using MonadicBits;
using NUnit.Framework;
using static MonadicBits.Functional;

namespace MonadicBitsTests
{
    public static class TryTests
    {
        [Test]
        public static void Test()
        {
            ((Func<int>)(() => 5))
                .Try(new[] { typeof(ArgumentException), typeof(DivideByZeroException) })
                .Match(_ => Assert.Pass(), _ => Assert.Fail());



            Try<object>(
                new[]
                {
                    typeof(ArgumentException),
                    typeof(DivideByZeroException)
                },
                () => throw new DivideByZeroException()
            ).Match(_ => Assert.Fail(), Assert.IsInstanceOf<DivideByZeroException>);
        }
    }
}