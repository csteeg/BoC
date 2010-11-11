using System;
using System.Collections.Generic;
using BoC.Extensions;
using Xunit;
using Xunit.Extensions;

namespace BoC.Tests.Extensions
{
    public class TypeExtensionsFixture
    {
        [Fact]
        public void IsGenericAssignableFrom_Should_Throw_If_Argument_Null()
        {
            Assert.Throws<ArgumentNullException>(() => TypeExtensions.IsGenericAssignableFrom(null, typeof (object)));
        }

        [Fact]
        public void IsGenericAssignableFrom_Should_Throw_If_Other_Argument_Null()
        {
            Assert.Throws<ArgumentNullException>(() => TypeExtensions.IsGenericAssignableFrom(typeof(IGenericInterface<>), null));
        }
        [Fact]
        public void IsGenericAssignableFrom_Should_Throw_If_Type_Is_NonGeneric()
        {
            Assert.Throws<ArgumentException>(() => typeof(object).IsGenericAssignableFrom(typeof(object)));
        }
        
        [Theory]
        [InlineData(typeof(IGenericInterface<>), typeof(IInheritedInterface), true)]
        [InlineData(typeof(IGenericInterface<>), typeof(ImplementedClass), true)]
        [InlineData(typeof(IGenericInterface<>), typeof(InheritedImplementedClass), true)]
        [InlineData(typeof(IGenericInterface<>), typeof(OtherImplementedClass), true)]
        [InlineData(typeof(IGenericInterface<>), typeof(object), false)]
        public void IsGenericAssignableFrom_Should_Check_If_Type_IsAssignableFrom(Type input, Type other, bool expect)
        {
            Console.WriteLine("Checking if type {0} isassignablefrom {1} (expecting {2})", input, other, expect);
            Assert.Equal(expect, TypeExtensions.IsGenericAssignableFrom(input, other));
        }
    }

    public interface IGenericInterface<T>{}
    public interface IInheritedInterface: IGenericInterface<Object>{}
    public class ImplementedClass: IGenericInterface<Object>{}
    public class InheritedImplementedClass : ImplementedClass{}
    public class OtherImplementedClass: IInheritedInterface{}
}
