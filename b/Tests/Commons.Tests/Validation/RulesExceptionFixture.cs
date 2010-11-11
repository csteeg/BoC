using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoC.Validation;
using Xunit;

namespace BoC.Tests.Validation
{
    public class RulesExceptionFixture
    {
        [Fact]
        public void Constructor_Should_Set_Property()
        {
            var errorInfo = new[] { new ErrorInfo("Dummy", "Error!") };
            var exception = new RulesException(errorInfo);
            
            Assert.Equal(exception.Errors, errorInfo);
        }

        [Fact]
        public void Constructor_Should_Create_ErrorInfo_List()
        {
            var onObject = new object();
            var exception = new RulesException("Dummy", "Error!", onObject);

            Assert.Equal(1, exception.Errors.Count());
            Assert.Equal("Dummy", exception.Errors.First().PropertyName);
            Assert.Equal("Error!", exception.Errors.First().ErrorMessage);
            Assert.Equal(onObject, exception.Errors.First().Object);
        }
        
        [Fact]
        public void Constructor_Should_Create_ErrorInfo_List_Without_Object()
        {
            var exception = new RulesException("Dummy", "Error!");

            Assert.Equal(1, exception.Errors.Count());
            Assert.Equal("Dummy", exception.Errors.First().PropertyName);
            Assert.Equal("Error!", exception.Errors.First().ErrorMessage);
            Assert.Equal(null, exception.Errors.First().Object);
        }
    }
}
