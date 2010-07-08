using System;
using System.Text;
using BoC.Extensions;
using Xunit;
using Xunit.Extensions;

namespace BoC.Tests.Extensions
{
    public class StringExtensionFixture
    {
        [Theory]
        [InlineData("http://dotnetshoutout.com", true)]
        [InlineData("htp://dotnetshoutout.com", false)]
        [InlineData("http://www.dotnetshoutout.com", true)]
        [InlineData("www.dotnetshoutout.com", false)]
        [InlineData("", false)]
        public void IsWebUrl_Should_Return_Correct_Result(string target, bool result)
        {
            Assert.Equal(result, target.IsWebUrl());
        }

        [Theory]
        [InlineData("admin@dotnetshoutout.com", true)]
        [InlineData("admin@dotnetshoutout.com.bd", true)]
        [InlineData("admin@dotnetshoutoutcom", false)]
        [InlineData("admin", false)]
        [InlineData("", false)]
        public void IsEmail_Should_Return_Correct_Result(string target, bool result)
        {
            Assert.Equal(result, target.IsEmail());
        }

        [Fact]
        public void NullSafe_Should_Return_Empty_String_When_Null_String_Is_Passed()
        {
            const string nullString = null;

            Assert.Equal(string.Empty, nullString.NullSafe());
        }

        [Fact]
        public void FormatWith_Should_Replace_Place_Holder_Tokens_With_Provided_Value()
        {
            Assert.Equal("A-B-C-D", "{0}-{1}-{2}-{3}".FormatWith("A", "B", "C", "D"));
        }

        [Theory]
        [InlineData('x', 2048, "cfb767f225d58469c5de3632a8803958")]
        public void MD5_Should_Return_Hashed_Value(char input, int length, string result)
        {
            var plain = new string(input, length);
            var hash = plain.MD5();

            Assert.Equal(result, hash);
        }

        [Theory]
        [InlineData("abcd")]
        [InlineData("a dummy string")]
        [InlineData("another dummy string")]
        [InlineData("x")]
        public void Hash_Should_Always_Return_Thirty_Two_Character_String(string target)
        {
            var hash = target.MD5();

            Assert.Equal(32, hash.Length);
        }

        public enum testEnum
        {
            invalid,
            foo,
            bar
        }
        [Theory]
        [InlineData("foo", testEnum.foo, testEnum.invalid)]
        [InlineData("bar", testEnum.bar, testEnum.invalid)]
        public void ToEnum_Should_Be_Able_To_Convert_From_String(string input, testEnum expect, testEnum defaultValue)
        {
            Assert.Equal(expect, input.ToEnum(defaultValue));
        }

        [Theory]
        [InlineData("nonexisting1", testEnum.invalid)]
        [InlineData("nonexisting2", testEnum.invalid)]
        public void ToEnum_Should_Fall_Back_To_Default(string input, testEnum output)
        {
            Assert.Equal(output, input.ToEnum(output));
        }

        [Theory]
        [InlineData("asdfnadfsf*(*FD DFK FD (DFDFKJ", "asdfnadfsfFDDFKFDDFDFKJ")]
        [InlineData("878&(*&abc .asdf", "878abcasdf")]
        public void LevenshteinDistance_Should_Say_Zero_When_Removing_Special_Chars(string input, string compare)
        {
            Assert.Equal(0, input.LevenshteinDistance(compare, true));
        }


        [Theory]
        [InlineData("asdfnadfsf*(*FD DFK FD (DFDFKJ")]
        [InlineData("878&(*&abc .asdf")]
        public void LevenshteinDistance_Should_Say_Zero_When_Strings_Are_Equal(string input)
        {
            Assert.Equal(0, input.LevenshteinDistance(input, false));
        }

        [Theory]
        [InlineData("", "", 0)]
        [InlineData("a", "", 1)]
        [InlineData("", "a", 1)]
        [InlineData("", "abc", 3)]
        [InlineData("abc", "", 3)]
        public void LevenshteinDistance_Should_Work_On_Empty_Strings(string input, string compare, int expect)
        {
            Assert.Equal(expect, input.LevenshteinDistance(compare));
        }

        [Theory]
        [InlineData("", "a", 1)]
        [InlineData("a", "ab", 1)]
        [InlineData("b", "ab", 1)]
        [InlineData("ac", "abc", 1)]
        [InlineData("abcdefg", "xabxcdxxefxgx", 6)]
        public void LevenshteinDistance_Should_Work_Where_Only_Inserts_Are_Needed(string input, string compare, int expect)
        {
            Assert.Equal(expect, input.LevenshteinDistance(compare));
        }

        [Theory]
        [InlineData("a", "", 1)]
        [InlineData("ab", "a", 1)]
        [InlineData("ab", "b", 1)]
        [InlineData("abc", "ac", 1)]
        [InlineData("xabxcdxxefxgx", "abcdefg", 6)]
        public void LevenshteinDistance_Should_Work_Where_Only_Deletes_Are_Needed(string input, string compare, int expect)
        {
            Assert.Equal(expect, input.LevenshteinDistance(compare));
        }

        [Theory]
        [InlineData("a", "b", 1)]
        [InlineData("ab", "ac", 1)]
        [InlineData("ac", "bc", 1)]
        [InlineData("abc", "axc", 1)]
        [InlineData("xabxcdxxefxgx", "1ab2cd34ef5g6", 6)]
        public void LevenshteinDistance_Should_Work_Where_Only_Substitutions_Are_Needed(string input, string compare, int expect)
        {
            Assert.Equal(expect, input.LevenshteinDistance(compare));
        }

        [Theory]
        [InlineData("example", "samples", 3)]
        [InlineData("sturgeon", "urgently", 6)]
        [InlineData("levenshtein", "frankenstein", 6)]
        [InlineData("distance", "difference", 5)]
        public void LevenshteinDistance_Should_Work_Where_Many_Operations_Are_Needed(string input, string compare, int expect)
        {
            Assert.Equal(expect, input.LevenshteinDistance(compare));
        }

        [Theory]
        [InlineData("example", "samples", (3*100)/7)]
        [InlineData("sturgeon", "urgently", (6*100)/8)]
        [InlineData("levenshtein", "frankenstein", 50)]
        [InlineData("distance", "difference", 50)]
        [InlineData("distance", "", 100)]
        [InlineData("a", "a", 0)]
        [InlineData("", "", 0)]
        [InlineData("a", "b", 100)]
        public void LevenshteinDistancePercentage_Should_Work_Where_Many_Operations_Are_Needed(string input, string compare, int expect)
        {
            Assert.Equal(expect, input.LevenshteinDistancePercentage(compare));
        }

    }
}