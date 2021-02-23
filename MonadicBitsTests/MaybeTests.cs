using MonadicBits;
using NUnit.Framework;

namespace MonadicBitsTests
{
    public class MaybeTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void MaybeCreateJust()
        {
            // Arrange
            const string input = "Test";

            // Act
            var resultLong = Maybe<string>.Just(input);
            var resultShort = input.Just();

            // Assert
            resultShort.Match(s => Assert.AreEqual(input, s), Assert.Fail);
            resultLong.Match(s => Assert.AreEqual(input, s), Assert.Fail);
        }

        [Test]
        public void JustNotNull()
        {
            // Arrange
            const string value = "Test";

            // Act
            var result = value.JustNotNull();
            var nullResult = ((string) null).JustNotNull();

            // Assert
            result.Match(s => Assert.AreEqual(value, s), Assert.Fail);
            nullResult.Match(_ => Assert.Fail(), Assert.Pass);
        }
    }
}
