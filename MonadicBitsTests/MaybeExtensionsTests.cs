using System;
using System.Collections.Generic;
using MonadicBits;
using NUnit.Framework;

namespace MonadicBitsTests
{
    public static class MaybeExtensionsTests
    {
        [Test]
        public static void Just_creates_maybe_with_value()
        {
            const string input = "Test";
            input.Just().Match(s => Assert.AreEqual(input, s), Assert.Fail);
        }

        [Test]
        public static void JustNotNull_creates_empty_maybe_from_null() =>
            ((string) null).JustNotNull().Match(Assert.Fail, Assert.Pass);

        [Test]
        public static void JustNotNull_creates_maybe_with_value_from_not_null()
        {
            const string value = "Test";
            value.JustNotNull().Match(s => Assert.AreEqual(value, s), Assert.Fail);
        }

        [Test]
        public static void FirstOrNothing_with_list_of_values_returns_maybe_of_first_value()
        {
            List<int> values = new() {23, 42, 15};
            values.FirstOrNothing().Match(i => Assert.AreEqual(values[0], i), Assert.Fail);
        }

        [Test]
        public static void FirstOrNothing_with_empty_list_returns_nothing() =>
            new List<int>().FirstOrNothing().Match(_ => Assert.Fail(), Assert.Pass);

        [Test]
        public static void JustWhen_with_null_predicate_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".JustWhen(null));
        
        [Test]
        public static void JustWhen_with_false_predicate_returns_nothing()
        {
            const string input = "Test";
            input.JustWhen(s => s.Length == input.Length + 1).Match(_ => Assert.Fail(), Assert.Pass);
        }
        
        [Test]
        public static void JustWhen_with_true_predicate_returns_just()
        {
            const string input = "Test";
            input.JustWhen(s => s.Length == input.Length).Match(_ => Assert.Pass(), Assert.Fail);
        }
    }
}