using System;
using FluentAssertions;
using MonadicBits;
using MonadicBits.Maybe;
using NUnit.Framework;
using static MonadicBitsTests.TestMonads;

namespace MonadicBitsTests
{
    public static class MaybeTests
    {
        [Test]
        public static void Just_creates_maybe_with_value()
        {
            const string input = "Test";
            input.Just().Should().Be(input);
        }

        [Test]
        public static void Nothing_creates_empty_maybe() =>
            Nothing<string>().Should().Be(Functional.Nothing);

        [Test]
        public static void Match_with_null_just_action_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Just().Match(null, () => { }));

        [Test]
        public static void Match_with_null_nothing_action_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Just().Match(_ => { }, null));

        [Test]
        public static void Match_with_null_just_func_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Just().Match(null, () => "Nothing"));

        [Test]
        public static void Match_with_null_nothing_func_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Just().Match(_ => "Just", null));

        [Test]
        public static void Match_maybe_with_value_returns_value()
        {
            const string value = "Test";
            Assert.AreEqual(value, value.Just().Match(v => v, () => "Nothing"));
        }

        [Test]
        public static void Match_empty_maybe_returns_nothing_value()
        {
            const string value = "Test";
            Assert.AreEqual(value, Nothing<string>().Match(_ => "Just", () => value));
        }

        [Test]
        public static void Match_maybe_with_value_calls_just_action() =>
            JustAnInt().Match(_ => Assert.Pass(), Assert.Fail);

        [Test]
        public static void Match_empty_maybe_calls_nothing_action() =>
            Nothing<string>().Match(Assert.Fail, Assert.Pass);

        [Test]
        public static void Map_with_null_mapping_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Just().Map((Func<string, string>)null));

        [Test]
        public static void Map_maybe_with_value_returns_maybe_with_new_type_and_value()
        {
            const int mappedValue = 42;
            "Test".Just().Map(_ => mappedValue)
                .Should().Be(mappedValue.Just());
        }

        [Test]
        public static void Map_empty_maybe_returns_empty_maybe() =>
            Nothing<string>().Map(_ => 42).Should().Be(Functional.Nothing);

        [Test]
        public static void Bind_to_null_method_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Just().Bind((Func<string, Maybe<string>>)null));

        [Test]
        public static void Bind_maybe_with_value_to_method_returns_maybe()
        {
            const int bindValue = 42;
            "Test".Just().Bind(_ => bindValue.Just())
                .Should().Be(bindValue.Just());
        }

        [Test]
        public static void Bind_empty_maybe_to_method_returns_empty_maybe() =>
            Nothing<string>().Bind(_ => 42.Just()).Should().Be(Functional.Nothing);

        [Test]
        public static void Just_to_either_makes_right()
        {
            const string input = "Test";
            var result = input.Just().ToEither("Left");
            result.Match(Assert.Fail, right => Assert.AreEqual(input, right));
        }

        [Test]
        public static void Nothing_to_either_makes_left()
        {
            const string leftInput = "Left";
            var result = Nothing<string>().ToEither(leftInput);
            result.Match(left => Assert.AreEqual(leftInput, left), Assert.Fail);
        }

        [Test]
        public static void String_representation_of_just_contains_value() =>
            42.Just().ToString().Should().Contain(42.ToString());

        [Test]
        public static void String_representation_of_nothing_is_constant() =>
            Nothing<int>().ToString().Should().Be(Nothing<string>().ToString());

        [Test]
        public static void Hash_code_of_just_is_hash_code_of_value() =>
            "test".Just().GetHashCode().Should().Be("test".GetHashCode());

        [Test]
        public static void Hash_code_of_nothing_is_hash_code_of_explicit_nothing() =>
            Nothing<int>().GetHashCode().Should().Be(new Nothing().GetHashCode());

        [Test]
        public static void Nothing_maybe_equals_explicit_nothing() =>
            Nothing<int>().Should().Be(Functional.Nothing);

        [Test]
        public static void Explicit_nothing_equals_nothing_maybe_of_arbitrary_type() =>
            Functional.Nothing.Should().Be(Nothing<int>());

        [Test]
        public static void Explicit_nothing_does_not_equal_null() =>
            Functional.Nothing.Should().NotBe(null);

        [Test]
        public static void Nothing_does_not_equal_null() =>
            Nothing<int>().Should().NotBe(null);

        [Test]
        public static void Just_equals_other_just_of_same_value() =>
            42.Just().Should().Be(42.Just());

        [Test]
        public static void Just_does_not_equal_other_just_of_same_type() =>
            42.Just().Should().NotBe(1.Just());

        [Test]
        public static void Just_does_not_equal_other_value_of_arbitrary_type() =>
            42.Just().Should().NotBe(new object());

        [Test]
        public static void Just_does_not_equal_nothing() =>
            42.Just().Should().NotBe(Nothing<int>());

        [Test]
        public static void Just_does_not_equal_explicit_nothing() =>
            42.Just().Should().NotBe(Functional.Nothing);
    }
}