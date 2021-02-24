using MonadicBits;
using NUnit.Framework;

namespace MonadicBitsTests
{
    public sealed class EitherTests
    {
        [Test]
        public void EitherCreate()
        {
            // Arrange
            const string input = "Test";

            // Act
            var leftResult = input.Left<string, string>();
            var rightResult = input.Right<string, string>();

            // Assert
            leftResult.Match(left => Assert.AreEqual(input, left), Assert.Fail);
            rightResult.Match(Assert.Fail, right => Assert.AreEqual(input, right));
        }

        [Test]
        public void Match()
        {
            // Arrange
            const string input = "Test";
            const string fail = "Fail";

            // Act
            var leftMatch = input.Left<string, string>().Match(left => left, _ => fail);
            var rightMatch = input.Right<string, string>().Match(_ => fail, right => right);
            var leftFailMatch = input.Right<string, string>().Match(left => left, _ => fail);
            var rightFailMatch = input.Left<string, string>().Match(_ => fail, right => right);

            // Assert
            Assert.AreEqual(input, leftMatch);
            Assert.AreEqual(input, rightMatch);
            Assert.AreEqual(fail, leftFailMatch);
            Assert.AreEqual(fail, rightFailMatch);
        }

        [Test]
        public void Map()
        {
            // Arrange
            const string input = "Test";
            const string mapping = "42";

            // Act
            var leftResult = input.Left<string, string>().MapLeft(s => $"{s}, {mapping}");
            var rightResult = input.Right<string, string>().Map(s => $"{s}, {mapping}");

            // Assert
            leftResult.Match(s => Assert.AreEqual($"{input}, {mapping}", s), Assert.Fail);
            rightResult.Match(Assert.Fail, s => Assert.AreEqual($"{input}, {mapping}", s));
        }

        [Test]
        public void Bind()
        {
            // Arrange
            const string input = "Test";
            const int mapping = 42;

            // Act
            var leftResult = input.Left<string, string>().BindLeft(_ => mapping.Left<int, string>());
            var rightResult = input.Right<string, string>().Bind(_ => mapping.Right<string, int>());

            // Assert
            leftResult.Match(i => Assert.AreEqual(mapping, i), Assert.Fail);
            rightResult.Match(Assert.Fail, i => Assert.AreEqual(mapping, i));
        }

        [Test]
        public void ToMaybe()
        {
            // Arrange
            const string leftInput = "Left";
            const string rightInput = "Right";

            // Act
            var leftResult = leftInput.Left<string, string>().ToMaybe();
            var rightResult = rightInput.Right<string, string>().ToMaybe();

            // Assert
            leftResult.Match(Assert.Fail, Assert.Pass);
            rightResult.Match(s => Assert.AreEqual(rightInput, s), Assert.Fail);
        }
    }
}
