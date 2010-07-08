using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoC.InversionOfControl;
using Xunit;
using Moq;

namespace BoC.Tests.InversionOfControl
{
    public class IoCFixture
    {
        [Fact]
        public void InitializeWith_Should_Set_Static_Resolver()
        {
            var depResolver = new Mock<IDependencyResolver>();

            IoC.InitializeWith(depResolver.Object);

            Assert.Equal(depResolver.Object, IoC.Resolver);
        }
    }
}
