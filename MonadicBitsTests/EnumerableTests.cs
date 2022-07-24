using FluentAssertions;
using NUnit.Framework;

namespace MonadicBitsTests
{
    public static class EnumerableTests
    {
        [Test]
        public static void Elevation_creates_single_item_enumeration() =>
            MonadicBits.MonadicEnumerable.Return(42).Should().BeEquivalentTo(new[] { 42 });
    }
}