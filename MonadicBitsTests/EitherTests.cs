using System;
using MonadicBits;
using NUnit.Framework;

namespace MonadicBitsTests
{
    public static class EitherTests
    {
        [Test]
        public static void Right_creates_either_with_right_value()
        {
            const string input = "Test";
            Either<string, string>.Right(input).Match(Assert.Fail, s => Assert.AreEqual(input, s));
            input.Right<string, string>().Match(Assert.Fail, s => Assert.AreEqual(input, s));
        }

        [Test]
        public static void Left_creates_either_with_left_value()
        {
            const string input = "Test";
            Either<string, string>.Left(input).Match(s => Assert.AreEqual(input, s), Assert.Fail);
            input.Left<string, string>().Match(s => Assert.AreEqual(input, s), Assert.Fail);
        }

        [Test]
        public static void Match_with_null_left_action_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Left<string, string>().Match(null, _ => { }));

        [Test]
        public static void Match_with_null_right_action_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Left<string, string>().Match(_ => { }, null));

        [Test]
        public static void Match_with_null_left_func_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Left<string, string>().Match(null, _ => "Right"));

        [Test]
        public static void Match_with_null_right_func_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Left<string, string>().Match(_ => "Left", null));

        [Test]
        public static void Match_right_either_returns_right_value()
        {
            const string value = "Test";
            var result = value.Right<string, string>().Match(_ => "Left", s => s);
            Assert.AreEqual(value, result);
        }

        [Test]
        public static void Match_left_either_returns_left_value()
        {
            const string value = "Test";
            var result = value.Left<string, string>().Match(s => s, _ => "Right");
            Assert.AreEqual(value, result);
        }

        [Test]
        public static void Map_right_either_with_null_mapping_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Right<string, string>().Map((Func<string, string>) null));

        [Test]
        public static void Map_right_either_returns_either_with_new_type_and_value()
        {
            const int mappedValue = 42;
            "Test".Right<string, string>().Map(_ => mappedValue)
                .Match(Assert.Fail, i => Assert.AreEqual(mappedValue, i));
        }

        [Test]
        public static void Map_left_either_returns_same_left_either()
        {
            const string value = "Test";
            value.Left<string, string>().Map(_ => 42).Match(s => Assert.AreEqual(value, s), _ => Assert.Fail());
        }

        [Test]
        public static void MapLeft_left_either_with_null_mapping_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() =>
                "Test".Left<string, string>().MapLeft((Func<string, string>) null));

        [Test]
        public static void MapLeft_left_either_returns_either_with_new_type_and_value()
        {
            const int mappedValue = 42;
            "Test".Left<string, string>().MapLeft(_ => mappedValue)
                .Match(i => Assert.AreEqual(mappedValue, i), Assert.Fail);
        }

        [Test]
        public static void MapLeft_right_either_returns_same_right_either()
        {
            const string value = "Test";
            value.Right<string, string>().MapLeft(_ => 42).Match(_ => Assert.Fail(), s => Assert.AreEqual(value, s));
        }

        [Test]
        public static void Bind_right_either_with_null_mapping_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() =>
                "Test".Right<string, string>().Bind((Func<string, Either<string, string>>) null));

        [Test]
        public static void Bind_right_either_to_method_returns_either_with_new_type_and_value()
        {
            const int bindValue = 42;
            "Test".Right<string, string>().Bind(_ => bindValue.Right<string, int>())
                .Match(Assert.Fail, i => Assert.AreEqual(bindValue, i));
        }

        [Test]
        public static void Bind_left_either_to_method_returns_same_left_either()
        {
            const string value = "Test";
            value.Left<string, string>().Bind(_ => 42.Right<string, int>())
                .Match(s => Assert.AreEqual(value, s), _ => Assert.Fail());
        }

        [Test]
        public static void BindLeft_left_either_with_null_mapping_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() =>
                "Test".Left<string, string>().BindLeft((Func<string, Either<string, string>>) null));

        [Test]
        public static void BindLeft_left_either_to_method_returns_either_with_new_type_and_value()
        {
            const int bindValue = 42;
            "Test".Left<string, string>().BindLeft(_ => bindValue.Left<int, string>())
                .Match(i => Assert.AreEqual(bindValue, i), Assert.Fail);
        }

        [Test]
        public static void BindLeft_right_either_to_method_returns_same_right_either()
        {
            const string value = "Test";
            value.Right<string, string>().BindLeft(_ => 42.Left<int, string>())
                .Match(_ => Assert.Fail(), s => Assert.AreEqual(value, s));
        }

        [Test]
        public static void Left_to_maybe_returns_empty_maybe() =>
            "Test".Left<string, string>().ToMaybe().Match(Assert.Fail, Assert.Pass);

        [Test]
        public static void Right_to_maybe_returns_maybe_with_value()
        {
            const string value = "Test";
            value.Right<string, string>().ToMaybe().Match(s => Assert.AreEqual(value, s), Assert.Fail);
        }

        [Test]
        public static void Select_with_null_selector_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() =>
                "Test".Right<string, string>().Select((Func<string, string>) null));

        [Test]
        public static void Select_from_maybe_with_value_returns_maybe_with_value()
        {
            const string input = "Test";
            (from s in input.Right<string, string>() select s).Match(Assert.Fail, s => Assert.AreEqual(input, s));
        }

        [Test]
        public static void SelectMany_with_null_collection_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => "Test".Right<string, string>().SelectMany(
                (Func<string, Either<string, string>>) null, (i, c) => $"{i}{c}"));

        [Test]
        public static void SelectMany_with_null_selector_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() =>
                "Test".Right<string, string>()
                    .SelectMany(s => s.Right<string, string>(), (Func<string, string, string>) null));

        [Test]
        public static void SelectMany_from_either_with_right_value_returns_either_with_right_value()
        {
            const int input = 42;
            (
                from s in "Test".Right<string, string>()
                from i in input.Right<string, int>()
                select i
            ).Match(Assert.Fail, i => Assert.AreEqual(input, i));
        }
    }
}