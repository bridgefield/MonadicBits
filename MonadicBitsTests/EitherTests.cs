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
            TestMonads.Right(input).Match(Assert.Fail, s => Assert.AreEqual(input, s));
        }

        [Test]
        public static void Left_creates_either_with_left_value()
        {
            const string input = "Test";
            TestMonads.Left(input).Match(s => Assert.AreEqual(input, s), Assert.Fail);
        }

        [Test]
        public static void Match_with_null_left_action_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => TestMonads.Left("Test").Match(null, _ => { }));

        [Test]
        public static void Match_with_null_right_action_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => TestMonads.Left("Test").Match(_ => { }, null));

        [Test]
        public static void Match_with_null_left_func_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => TestMonads.Left("Test").Match(null, _ => "Right"));

        [Test]
        public static void Match_with_null_right_func_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => TestMonads.Left("Test").Match(_ => "Left", null));

        [Test]
        public static void Match_right_either_returns_right_value()
        {
            const string value = "Test";
            var result = TestMonads.Right(value).Match(_ => "Left", s => s);
            Assert.AreEqual(value, result);
        }

        [Test]
        public static void Match_left_either_returns_left_value()
        {
            const string value = "Test";
            var result = TestMonads.Left(value).Match(s => s, _ => "Right");
            Assert.AreEqual(value, result);
        }

        [Test]
        public static void Map_right_either_with_null_mapping_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() => TestMonads.Right("Test").Map((Func<string, string>)null));

        [Test]
        public static void Map_right_either_returns_either_with_new_type_and_value()
        {
            const int mappedValue = 42;
            TestMonads.Right("Test").Map(_ => mappedValue)
                .Match(Assert.Fail, i => Assert.AreEqual(mappedValue, i));
        }

        [Test]
        public static void Map_left_either_returns_same_left_either()
        {
            const string value = "Test";
            TestMonads.Left(value).Map(_ => 42).Match(s => Assert.AreEqual(value, s), _ => Assert.Fail());
        }

        [Test]
        public static void MapLeft_left_either_with_null_mapping_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() =>
                TestMonads.Left("Test").MapLeft((Func<string, string>)null));

        [Test]
        public static void MapLeft_left_either_returns_either_with_new_type_and_value()
        {
            const int mappedValue = 42;
            TestMonads.Left("Test").MapLeft(_ => mappedValue)
                .Match(i => Assert.AreEqual(mappedValue, i), Assert.Fail);
        }

        [Test]
        public static void MapLeft_right_either_returns_same_right_either()
        {
            const string value = "Test";
            TestMonads.Right(value).MapLeft(_ => 42).Match(_ => Assert.Fail(), s => Assert.AreEqual(value, s));
        }

        [Test]
        public static void Bind_right_either_with_null_mapping_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() =>
                TestMonads.Right("Test").Bind((Func<string, Either<string, string>>)null));

        [Test]
        public static void Bind_right_either_to_method_returns_either_with_new_type_and_value()
        {
            const int bindValue = 42;
            TestMonads.Right("Test").Bind(_ => bindValue.Right<string, int>())
                .Match(Assert.Fail, i => Assert.AreEqual(bindValue, i));
        }

        [Test]
        public static void Bind_left_either_to_method_returns_same_left_either()
        {
            const string value = "Test";
            TestMonads.Left(value).Bind(_ => 42.Right<string, int>())
                .Match(s => Assert.AreEqual(value, s), _ => Assert.Fail());
        }

        [Test]
        public static void BindLeft_left_either_with_null_mapping_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() =>
                TestMonads.Left("Test").BindLeft((Func<string, Either<string, string>>)null));

        [Test]
        public static void BindLeft_left_either_to_method_returns_either_with_new_type_and_value()
        {
            const int bindValue = 42;
            TestMonads.Left("Test").BindLeft(_ => bindValue.Left<int, string>())
                .Match(i => Assert.AreEqual(bindValue, i), Assert.Fail);
        }

        [Test]
        public static void BindLeft_right_either_to_method_returns_same_right_either()
        {
            const string value = "Test";
            TestMonads.Right(value).BindLeft(_ => 42.Left<int, string>())
                .Match(_ => Assert.Fail(), s => Assert.AreEqual(value, s));
        }

        [Test]
        public static void Left_to_maybe_returns_empty_maybe() =>
            TestMonads.Left("Test").ToMaybe().Match(Assert.Fail, Assert.Pass);

        [Test]
        public static void Right_to_maybe_returns_maybe_with_value()
        {
            const string value = "Test";
            TestMonads.Right(value).ToMaybe().Match(s => Assert.AreEqual(value, s), Assert.Fail);
        }
    }
}