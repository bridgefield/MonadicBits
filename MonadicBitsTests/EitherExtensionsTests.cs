using MonadicBits;
using NUnit.Framework;

namespace MonadicBitsTests
{
    public static class EitherExtensionsTests
    {
        [Test]
        public static void Right_creates_either_with_right_value()
        {
            const string input = "Test";
            input.Right<string, string>().Match(Assert.Fail, s => Assert.AreEqual(input, s));
        }

        [Test]
        public static void Left_creates_either_with_left_value()
        {
            const string input = "Test";
            input.Left<string, string>().Match(s => Assert.AreEqual(input, s), Assert.Fail);
        }
    }
}