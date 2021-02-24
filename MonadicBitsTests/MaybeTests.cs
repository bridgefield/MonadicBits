using MonadicBits;
using NUnit.Framework;

namespace MonadicBitsTests
{
    public sealed class MaybeTests
    {
        [Test]
        public void MaybeCreate()
        {
            // Arrange
            const string input = "Test";

            // Act
            var longResult = Maybe<string>.Just(input);
            var shortResult = input.Just();
            var nothingResult = Maybe<string>.Nothing();

            // Assert
            shortResult.Match(s => Assert.AreEqual(input, s), Assert.Fail);
            longResult.Match(s => Assert.AreEqual(input, s), Assert.Fail);
            nothingResult.Match(Assert.Fail, Assert.Pass);
        }

        [Test]
        public void JustNotNull()
        {
            // Arrange
            const string value = "Test";
            const string nullValue = null;

            // Act
            var result = value.JustNotNull();
            var nullResult = nullValue.JustNotNull();

            // Assert
            result.Match(s => Assert.AreEqual(value, s), Assert.Fail);
            nullResult.Match(Assert.Fail, Assert.Pass);
        }

        [Test]
        public void Match()
        {
            // Arrange
            const string value = "Test";
            const string nothing = "Nothing";
            var justResult = value.Just();
            var nothingResult = Maybe<string>.Nothing();

            // Act
            var matchJustResult = justResult.Match(v => v, () => nothing);
            var matchNothingResult = nothingResult.Match(v => v, () => nothing);

            // Act/Assert
            justResult.Match(Assert.Pass, Assert.Fail);
            nothingResult.Match(Assert.Fail, Assert.Pass);
            Assert.AreEqual(value, matchJustResult);
            Assert.AreEqual(nothing, matchNothingResult);
        }

        [Test]
        public void Map()
        {
            // Arrange
            const string input = "Test";
            const string mapping = "42";

            // Act
            var result = input.Just().Map(i => $"{i}, {mapping}");

            // Assert
            result.Match(s => Assert.AreEqual($"{input}, {mapping}", s), Assert.Fail);
        }

        [Test]
        public void Bind()
        {
            // Arrange
            const string input = "Test";
            const int mapping = 42;

            // Act
            var result = input.Just().Bind(_ => mapping.Just());

            // Assert
            result.Match(s => Assert.AreEqual(mapping, s), Assert.Fail);
        }

        [Test]
        public void ToEither()
        {
            // Arrange
            const string input = "Test";
            const string leftInput = "Left";

            // Act
            var justResult = input.Just().ToEither(leftInput);
            var nothingResult = Maybe<string>.Nothing().ToEither(leftInput);

            // Assert
            justResult.Match(Assert.Fail, right => Assert.AreEqual(input, right));
            nothingResult.Match(left => Assert.AreEqual(leftInput, left), Assert.Fail);
        }
    }
}
