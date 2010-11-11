using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoC.Validation;
using Xunit;

namespace BoC.Tests.Validation
{
    public class ErrorInfoFixture
    {
        [Fact]
        public void Constructor_Should_Set_All_3_Properties()
        {
            var onObject = new object();
            var errorInfo = new ErrorInfo("Dummy", "Error!", onObject);

            Assert.Equal(onObject, errorInfo.Object);
            Assert.Equal("Dummy", errorInfo.PropertyName);
            Assert.Equal("Error!", errorInfo.ErrorMessage);
        }

        [Fact]
        public void Constructor_Should_Set_Properties()
        {
            var errorInfo = new ErrorInfo("Dummy", "Error!");

            Assert.Equal(null, errorInfo.Object);
            Assert.Equal("Dummy", errorInfo.PropertyName);
            Assert.Equal("Error!", errorInfo.ErrorMessage);
        }
    }
}
