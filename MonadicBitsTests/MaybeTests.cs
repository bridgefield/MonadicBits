using System;
using MonadicBits;
using NUnit.Framework;

namespace MonadicBitsTests
{
    public static class MaybeTests
    {
        [Test]
        public static void Just_creates_maybe_with_value()
        {
            const string input = "Test";
            var longResult = Maybe<string>.Just(input);
            longResult.Match(s => Assert.AreEqual(input, s), Assert.Fail);
            var shortResult = input.Just();
            shortResult.Match(s => Assert.AreEqual(input, s), Assert.Fail);
        }

        [Test]
        public static void Nothing_creates_empty_maybe() =>
            Maybe<string>.Nothing().Match(Assert.Fail, Assert.Pass);

        [Test]
        public static void JustNotNull_creates_empty_maybe_from_null() =>
            ((string) null).JustNotNull().Match(Assert.Fail, Assert.Pass);

        [Test]
        public static void JustNotNull_creates_maybe_with_value_from_not_null()
        {
            const string value = "Test";
            var result = value.JustNotNull();
            result.Match(s => Assert.AreEqual(value, s), Assert.Fail);
        }

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
            var result = value.Just().Match(v => v, () => "Nothing");
            Assert.AreEqual(value, result);
        }

        [Test]
        public static void Match_empty_maybe_returns_nothing_value()
        {
            const string value = "Test";
            var result = Maybe<string>.Nothing().Match(_ => "Just", () => value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public static void Match_maybe_with_value_calls_just_action() =>
            "Test".Just().Match(Assert.Pass, Assert.Fail);

        [Test]
        public static void Match_empty_maybe_calls_nothing_action() =>
            Maybe<string>.Nothing().Match(Assert.Fail, Assert.Pass);

        [Test]
        public static void Map_with_null_mapping_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Just().Map((Func<string, string>) null));

        [Test]
        public static void Map_maybe_with_value_returns_maybe_with_new_type_and_value()
        {
            const int mappedValue = 42;
            "Test".Just().Map(_ => mappedValue).Match(i => Assert.AreEqual(mappedValue, i), Assert.Fail);
        }

        [Test]
        public static void Map_empty_maybe_returns_empty_maybe() =>
            Maybe<string>.Nothing().Map(_ => 42).Match(_ => Assert.Fail(), Assert.Pass);

        [Test]
        public static void Bind_to_null_method_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Just().Bind((Func<string, Maybe<string>>) null));

        [Test]
        public static void Bind_maybe_with_value_to_method_returns_maybe()
        {
            const int bindValue = 42;
            "Test".Just().Bind(_ => bindValue.Just()).Match(i => Assert.AreEqual(bindValue, i), Assert.Fail);
        }

        [Test]
        public static void Bind_empty_maybe_to_method_returns_empty_maybe() =>
            Maybe<string>.Nothing().Bind(_ => 42.Just()).Match(_ => Assert.Fail(), Assert.Pass);

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
            var result = Maybe<string>.Nothing().ToEither(leftInput);
            result.Match(left => Assert.AreEqual(leftInput, left), Assert.Fail);
        }

        [Test]
        public static void Select_with_null_selector_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Just().Select((Func<string, string>) null));

        [Test]
        public static void Select_from_maybe_with_value_returns_maybe_with_value()
        {
            const string input = "Test";
            (from s in input.Just() select s).Match(s => Assert.AreEqual(input, s), Assert.Fail);
        }

        [Test]
        public static void SelectMany_with_null_collection_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Just().SelectMany((Func<string, Maybe<string>>) null,
                (i, c) => $"{i}{c}"));

        [Test]
        public static void SelectMany_with_null_selector_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() =>
                "Test".Just().SelectMany(s => s.Just(), (Func<string, string, string>) null));

        [Test]
        public static void SelectMany_from_maybe_with_value_returns_maybe_with_value()
        {
            const int input = 42;
            (
                from s in "Test".Just()
                from i in input.Just()
                select i
            ).Match(i => Assert.AreEqual(input, i), Assert.Fail);
        }
    }
}