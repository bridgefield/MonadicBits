using System;
using FluentAssertions;
using MonadicBits;
using NUnit.Framework;

namespace MonadicBitsTests
{
    using static Functional;

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
            TestMonads.Right("Test")
                .Map(_ => mappedValue)
                .Should().Be(mappedValue.Right());
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
            TestMonads.Left("Test")
                .MapLeft(_ => mappedValue)
                .Should()
                .Be(mappedValue.Left());
        }

        [Test]
        public static void MapLeft_right_either_returns_same_right_either()
        {
            const string value = "Test";
            TestMonads.Right(value)
                .MapLeft(_ => 42)
                .Should()
                .Be(value.Right());
        }

        [Test]
        public static void Bind_right_either_with_null_mapping_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() =>
                TestMonads.Right("Test").Bind((Func<string, Either<string, string>>)null));

        [Test]
        public static void Bind_right_either_to_method_returns_either_with_new_type_and_value()
        {
            const int bindValue = 42;
            TestMonads.Right("Test")
                .Bind(_ => bindValue.Right<string, int>())
                .Should().Be(bindValue.Right());
        }

        [Test]
        public static void Bind_on_left_changes_right_type_only()
        {
            const string value = "Test";
            TestMonads.Left(value)
                .Bind(_ => 42.Right<string, int>())
                .Should().Be(value.Left<string, int>());
        }

        [Test]
        public static void BindLeft_left_either_with_null_mapping_throws_exception() =>
            Assert.Throws<ArgumentNullException>(() =>
                TestMonads.Left("Test").BindLeft((Func<string, Either<string, string>>)null));

        [Test]
        public static void BindLeft_on_left_changes_left_type_and_value()
        {
            const int bindValue = 42;
            TestMonads.Left("Test")
                .BindLeft(_ => bindValue.Left<int, string>())
                .Should().Be(bindValue.Left());
        }

        [Test]
        public static void BindLeft_on_right_changes_left_type_only()
        {
            const string value = "Test";
            TestMonads.Right(value)
                .BindLeft(_ => 42.Left<int, string>())
                .Should().Be(value.Right<int, string>());
        }

        [Test]
        public static void Left_to_maybe_returns_nothing() =>
            TestMonads.Left("Test").ToMaybe()
                .Should().Be(Nothing);

        [Test]
        public static void Right_to_maybe_returns_just()
        {
            const string value = "Test";
            TestMonads.Right(value)
                .ToMaybe().Should().Be(value.Just());
        }

        [Test]
        public static void BindLeft_on_explicit_left_changes_left_value_and_type() =>
            42.Left().BindLeft(v => v.ToString().Left<string, string>())
                .Should().Be(42.ToString().Left());

        [Test]
        public static void Bind_on_explicit_right_changes_value_and_type() =>
            42.Right().Bind(v => v.ToString().Right<int, string>())
                .Should().Be(42.ToString().Right());

        [Test]
        public static void Left_does_not_equal_arbitrary_right_of_same_type() =>
            42.Left<int, string>().Should().NotBe("right".Right<int, string>());

        [Test]
        public static void Left_does_not_equal_arbitrary_value_of_correct_right_type() =>
            42.Left<int, string>().Should().NotBe("right");

        [Test]
        public static void Right_does_not_equal_arbitrary_left_of_same_type() =>
            42.Right<string, int>().Should().NotBe("right".Left<string, int>());

        [Test]
        public static void Right_does_not_equal_arbitrary_value_of_correct_left_type() =>
            42.Right<string, int>().Should().NotBe("right");

        [Test]
        public static void Hash_codes_of_same_value_right_are_equal() =>
            42.Right<int, int>().GetHashCode().Should().Be(42.Right<int, int>().GetHashCode());

        [Test]
        public static void Hash_codes_of_same_value_left_are_equal() =>
            42.Left<int, int>().GetHashCode().Should().Be(42.Left<int, int>().GetHashCode());

        [Test]
        public static void Hash_codes_of_same_value_left_and_right_are_not_equal() =>
            42.Left<int, int>().GetHashCode().Should().NotBe(42.Right<int, int>().GetHashCode());

        [Test]
        public static void String_representation_of_left_contains_value_representation() =>
            42.Left<int, int>().ToString().Should().Contain(42.ToString());

        [Test]
        public static void String_representation_of_right_contains_value_representation() =>
            42.Right<int, int>().ToString().Should().Contain(42.ToString());

        [Test]
        public static void String_representation_of_right_and_left_with_same_value_and_type_are_not_equal() =>
            42.Right<int, int>().ToString()
                .Should().NotBe(42.Left<int, int>().ToString());
    }
}