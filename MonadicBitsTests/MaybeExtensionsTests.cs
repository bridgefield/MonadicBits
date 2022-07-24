using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using FluentAssertions;
using Microsoft.VisualBasic.CompilerServices;
using MonadicBits;
using NUnit.Framework;

namespace MonadicBitsTests
{
    using static Functional;

    public static class MaybeExtensionsTests
    {
        [Test]
        public static void Just_creates_maybe_with_value()
        {
            const string input = "Test";
            input.Just().Match(
                j => Assert.That(j, Is.EqualTo(input)),
                Assert.Fail);
        }

        [Test]
        public static void JustNotNull_creates_empty_maybe_from_null() =>
            ((string)null).JustNotNull().Should().Be(Nothing);

        [Test]
        public static void JustNotNull_creates_maybe_with_value_from_not_null()
        {
            const string value = "Test";
            value.JustNotNull().Should().Be(value.Just());
        }

        [Test]
        public static void ToMaybe_creates_nothing_from_empty_nullable() =>
            ((int?)null).ToMaybe().Should().Be(Nothing);

        [Test]
        public static void ToMaybe_creates_just_from_nullable_value() =>
            ((int?)1).ToMaybe().Should().Be(1.Just());

        [Test]
        public static void Just_to_nullable_returns_value() =>
            42.Just().ToNullable().Should().Be(42);

        [Test]
        public static void Nothing_to_nullable_returns_null() =>
            TestMonads.Nothing<int>().ToNullable().Should().Be(null);

        [Test]
        public static void Or_on_just_returns_initial_value()
        {
            const string initialValue = "value";
            initialValue.Just().Or(() => "alternative".Just())
                .Should().Be(initialValue.Just());
        }

        [Test]
        public static void Or_on_nothing_returns_alternative_value()
        {
            const string alternativeValue = "alternative";
            TestMonads.Nothing<string>().Or(() => alternativeValue.Just())
                .Should().Be(alternativeValue.Just());
        }

        [Test]
        public static void Or_on_nothing_and_nothing_alternative_returns_nothing() =>
            TestMonads.Nothing<string>().Or(() => Nothing)
                .Should().Be(Nothing);

        [Test]
        public static void Enumerable_created_from_just_contains_value()
        {
            const int value = 125;
            value.Just().ToEnumerable().Should().BeEquivalentTo(value.ToEnumerable());
        }

        [Test]
        public static void Enumerable_created_from_nothing_is_empty() =>
            TestMonads.Nothing<int>().ToEnumerable().Should().BeEmpty();

        [Test]
        public static void FirstOrNothing_with_list_of_values_returns_maybe_of_first_value()
        {
            List<int> values = new() { 23, 42, 15 };
            values.FirstOrNothing().Should().Be(values[0].Just());
        }

        [Test]
        public static void FirstOrNothing_with_predicate_returns_first_matching_item()
        {
            const int matchingValue = 25;
            var allValues = new[] { 1, 2, matchingValue, 34 };
            allValues.FirstOrNothing(v => v == matchingValue)
                .Should().Be(matchingValue.Just());
        }

        [Test]
        public static void FirstOrNothing_with_predicate_returns_nothing_when_no_item_matches() =>
            new[] { 1, 2, 34 }.FirstOrNothing(v => v == 25)
                .Should().Be(Nothing);

        [Test]
        public static void FirstOrNothing_with_empty_list_returns_nothing() =>
            new List<int>().FirstOrNothing().Should().Be(Nothing);

        [Test]
        public static void Just_throws_on_null() =>
            Assert.Throws<ArgumentNullException>(() => ((object)null).Just());

        [Test]
        public static void JustWhen_with_null_predicate_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".JustWhen(null));

        [Test]
        public static void JustWhen_with_false_predicate_returns_nothing()
        {
            const string input = "Test";
            input.JustWhen(s => s.Length == input.Length + 1)
                .Should().Be(Nothing);
        }

        [Test]
        public static void JustWhen_with_true_predicate_returns_just()
        {
            const string input = "Test";
            input.JustWhen(s => s.Length == input.Length)
                .Should().Be(input.Just());
        }
    }
}