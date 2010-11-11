using System;
using BoC.Helpers;
using Xunit;

namespace BoC.Tests.Helpers
{
    public class CheckArgumentFixture
    {
        [Fact]
        public void Constructor()
        {
        }

        [Fact]
        public void IsNotEmpty_With_Empty_Guid_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentException>(() => Check.Argument.IsNotEmpty(Guid.Empty, "Empty"));
        }

        [Fact]
        public void IsNotEmpty_With_NoNEmpty_Guid_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotEmpty(Guid.NewGuid(), "NonEmpty"));
        }

        [Fact]
        public void IsNotEmpty_With_Empty_String_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentException>(() => Check.Argument.IsNotEmpty(string.Empty, "Empty"));
        }

        [Fact]
        public void IsNotEmpty_With_NonEmpty_String_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotEmpty("AString", "NonEmpty"));
        }

        [Fact]
        public void IsNotOutOfLength_With_Length_Exceeding_String_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentException>(() => Check.Argument.IsNotOutOfLength("xx", 1, "lengthExceeding"));
        }

        [Fact]
        public void IsNotOutOfLength_With_Length_Not_Exceeding_String_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotOutOfLength("xx", 2, "lengthNotExceeding"));
        }

        [Fact]
        public void IsNotNull_With_Null_Object_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => Check.Argument.IsNotNull(null, "null"));
        }

        [Fact]
        public void IsNotNull_With_NonNull_Object_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotNull(new object(), "notnull"));
        }

        [Fact]
        public void IsNotNegative_With_Negative_Integer_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Argument.IsNotNegative(int.MinValue, "Negative"));
        }

        [Fact]
        public void IsNotNegative_With_Positive_Integer_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotNegative(int.MaxValue, "positive"));
        }

        [Fact]
        public void IsNotNegativeOrZero_With_Integer_Zero_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Argument.IsNotNegativeOrZero(0, "Zero"));
        }

        [Fact]
        public void IsNotNegativeOrZero_With_Positive_Integer_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotNegativeOrZero(int.MaxValue, "positive"));
        }

        [Fact]
        public void IsNotNegative_With_Negative_long_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Argument.IsNotNegative(long.MinValue, "Negative"));
        }

        [Fact]
        public void IsNotNegative_With_Positive_Long_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotNegative(long.MaxValue, "positive"));
        }

        [Fact]
        public void IsNotNegativeOrZero_With_Long_Zero_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Argument.IsNotNegativeOrZero(0L, "Zero"));
        }

        [Fact]
        public void IsNotNegativeOrZero_With_Positive_Long_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotNegativeOrZero(long.MaxValue, "positive"));
        }

        [Fact]
        public void IsNotNegative_With_Negative_float_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Argument.IsNotNegative(float.MinValue, "Negative"));
        }

        [Fact]
        public void IsNotNegative_With_Poaitive_Float_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotNegative(float.MaxValue, "positive"));
        }

        [Fact]
        public void IsNotNegativeOrZero_With_Float_Zero_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Argument.IsNotNegativeOrZero(0f, "Zero"));
        }

        [Fact]
        public void IsNotNegativeOrZero_Positive_Float_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotNegativeOrZero(float.MaxValue, "positive"));
        }

        [Fact]
        public void IsNotNegative_With_Negative_Decimal_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Argument.IsNotNegative(decimal.MinValue, "Negative"));
        }

        [Fact]
        public void IsNotNegative_With_Positive_Decimal_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotNegative(decimal.MaxValue, "positive"));
        }

        [Fact]
        public void IsNotNegativeOrZero_With_Decimal_Zero_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Argument.IsNotNegativeOrZero(0M, "Zero"));
        }

        [Fact]
        public void IsNotNegativeOrZero_With_Positive_Decimal_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotNegativeOrZero(decimal.MaxValue, "positive"));
        }

        [Fact]
        public void IsNotEmpty_With_Null_Array_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => Check.Argument.IsNotEmpty((int[])null, "null"));
        }

        [Fact]
        public void IsNotEmpty_With_Empty_Array_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentException>(() => Check.Argument.IsNotEmpty(new int[0], "empty"));
        }

        [Fact]
        public void IsNotEmpty_With_Populated_Array_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotEmpty(new[] { 1, 2, 3 }, "populated"));
        }

        [Fact]
        public void IsNotOutOfRange_With_Out_Of_Range_Value_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Argument.IsNotOutOfRange(11, 1, 10, "OutOfRange"));
        }

        [Fact]
        public void IsNotOutOfRange_With_In_Range_Value_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotOutOfRange(10, 1, 10, "InRange"));
        }

        [Fact]
        public void IsNotInPast_With_One_Second_Ago_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Argument.IsNotInPast(DateTime.Now.AddSeconds(-1), "OneSecondAgo"));
        }

        [Fact]
        public void IsNotInPast_With_OneSecond_After_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotInPast(DateTime.Now.AddSeconds(1), "OneSecondAfter"));
        }

        [Fact]
        public void IsNotInFuture_With_One_Second_After_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Argument.IsNotInFuture(DateTime.Now.AddSeconds(1), "OneSecondAfter"));
        }

        [Fact]
        public void IsNotInFuture_With_OneSecond_Ago_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotInFuture(DateTime.Now.AddSeconds(-1), "OneSecondAgo"));
        }

        [Fact]
        public void IsNotNegative_With_Minus_One_Second_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Argument.IsNotNegative(TimeSpan.FromSeconds(-1), "MinusOneSecond"));
        }

        [Fact]
        public void IsNotNegative_With_Zero_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotNegative(TimeSpan.Zero, "Zero"));
        }

        [Fact]
        public void IsNotNegativeOrZero_With_Zero_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Argument.IsNotNegativeOrZero(TimeSpan.Zero, "Zero"));
        }

        [Fact]
        public void IsNotNegativeOrZero_With_Plus_One_Second_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotNegativeOrZero(TimeSpan.FromSeconds(1), "PlusOneSecond"));
        }

        [Fact]
        public void IsNotInvalidEmail_With_Invalid_Email_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentException>(() => Check.Argument.IsNotInvalidEmail("xxx", "InvalidEmail"));
        }

        [Fact]
        public void IsNotInvalidEmail_With_Valid_Email_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotInvalidEmail("admin@kigg.com", "ValidEmail"));
        }

        [Fact]
        public void IsNotInvalidWebUrl_With_Invalid_Url_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentException>(() => Check.Argument.IsNotInvalidWebUrl("ftp://xxx.com", "InvalidWebUrl"));
        }

        [Fact]
        public void IsNotInvalidWebUrl_With_Valid_Url_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => Check.Argument.IsNotInvalidWebUrl("http://kigg.com", "ValidUrl"));
        }
    }
}