using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoC.Helpers;
using BoC.InversionOfControl;
using Xunit;
using Moq;

namespace BoC.Tests.InversionOfControl
{
    public class IoCFixture: BaseIoCFixture
    {
        public override void Dispose()
        {
            base.Dispose();

            IoC.Reset();
        }

        [Fact]
        public void InitializeWith_Should_Set_Static_Resolver()
        {
            var depResolver = new Mock<IDependencyResolver>();

            IoC.InitializeWith(depResolver.Object);

            Assert.Equal(depResolver.Object, IoC.Resolver);
            Assert.True(IoC.IsInitialized());
        }

        [Fact]
        public void InitializeWith_Does_Not_Allow_null()
        {
            Assert.Throws<ArgumentNullException>(() => IoC.InitializeWith(null));
        }

        [Fact]
        public void Reset_Should_Clear_Resolver()
        {
            var depResolver = new Mock<IDependencyResolver>();

            IoC.InitializeWith(depResolver.Object);
            IoC.Reset();
            Assert.Null(IoC.Resolver);
            Assert.False(IoC.IsInitialized());
        }

        [Fact]
        public void InitializeWith_Can_Be_Called_Only_Once()
        {
            var depResolver = new Mock<IDependencyResolver>();

            IoC.InitializeWith(depResolver.Object);
            Assert.Throws<ApplicationException>(() => IoC.InitializeWith(depResolver.Object));
        }

        [Fact]
        public void InitializeWith_Should_Call_ContainerInitializers_Execute()
        {
            var appDomainHelper = SetupResolve<IAppDomainHelper>();
            appDomainHelper.Setup(a => a.GetTypes(It.IsAny<Func<Type, bool>>())).Returns(() => new[] { typeof(TestableContainerInitializer) });
            resolver.Setup(r => r.ResolveAll<IAppDomainHelper>()).Returns(() => new[] {appDomainHelper.Object});
            
            var testableContainerInitializer = new TestableContainerInitializer();
            resolver.Setup(r => r.ResolveAll<IContainerInitializer>()).Returns(() => new[] { testableContainerInitializer });
            
            IoC.InitializeWith(resolver.Object);
            
            Assert.True(testableContainerInitializer.Executed);
        }

        [Fact]
        public void InitializeWith_Should_Call_User_ContainerInitializers_Execute_Before_Boc_Namespace()
        {
            var testableContainerInitializer = new TestableContainerInitializer();
            var user_testableContainerInitializer = new Mock<IContainerInitializer>();

            user_testableContainerInitializer.Setup(u => u.Execute()).Callback(
                () => Assert.False(testableContainerInitializer.Executed)).Verifiable(); //i'm being called first, so the boc. namespace one should be false

            var appDomainHelper = SetupResolve<IAppDomainHelper>();
            appDomainHelper.Setup(a => a.GetTypes(It.IsAny<Func<Type, bool>>())).Returns(() => new[]
                                                                                                   {
                                                                                                       user_testableContainerInitializer.Object.GetType(),
                                                                                                       typeof(TestableContainerInitializer)
                                                                                                   });
            
            resolver.Setup(r => r.ResolveAll<IAppDomainHelper>()).Returns(() => new[] { appDomainHelper.Object });
            
            resolver.Setup(r => r.ResolveAll<IContainerInitializer>()).Returns(() => new[] { testableContainerInitializer, user_testableContainerInitializer.Object });

            IoC.InitializeWith(resolver.Object);

            Assert.True(testableContainerInitializer.Executed);
            user_testableContainerInitializer.Verify();
        }

        public class TestableContainerInitializer : IContainerInitializer
        {
            internal bool Executed { get; set; }
            public void Execute()
            {
                Executed = true;
            }
        }
    }
}
