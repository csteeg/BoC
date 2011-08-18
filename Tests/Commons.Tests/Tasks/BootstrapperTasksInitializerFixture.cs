using System;
using System.Linq;
using BoC.Helpers;
using BoC.Tasks;
using Moq;
using Xunit;

namespace BoC.Tests.Tasks
{
    public class BootstrapperTasksInitializerFixture: BaseIoCFixture
    {
        [Fact]
        public void Run_Should_Execute_Tasks()
        {
            var task1 = new Mock<IBootstrapperTask>();
            task1.Setup(t => t.Execute()).Verifiable();
            resolver.Setup(r => r.ResolveAll<IBootstrapperTask>()).Returns(new[] { task1.Object });
            var appdomainHelper = new Mock<IAppDomainHelper>();

            new Bootstrapper(resolver.Object, new[] {appdomainHelper.Object}).Run();

            task1.Verify();
        }

        [Fact]
        public void Run_Should_Register_Tasks()
        {
            var task1 = new Mock<IBootstrapperTask>();
            
            resolver.Setup(r => r.ResolveAll<IBootstrapperTask>()).Returns(new[] { task1.Object });
            
            var appdomainHelper = new Mock<IAppDomainHelper>();
            appdomainHelper.Setup(h => h.GetTypes(It.IsAny<Func<Type, bool>>()))
                .Returns<Func<Type, bool>>(f => new Type[] { typeof(FindableBootstrapperTask), typeof(object) }.Where(f));

            new Bootstrapper(resolver.Object, new[] {appdomainHelper.Object}).Run();

            resolver.Verify(r => r.RegisterType(typeof(IBootstrapperTask), typeof(FindableBootstrapperTask)), Times.Once());
        }

        [Fact]
        public void Run_Should_Register_And_Run_Tasks()
        {
            var task1 = new Mock<IBootstrapperTask>();

            resolver.Setup(r => r.ResolveAll<IBootstrapperTask>()).Returns(new[] { task1.Object });

            var appdomainHelper = new Mock<IAppDomainHelper>();
            appdomainHelper.Setup(h => h.GetTypes(It.IsAny<Func<Type, bool>>()))
                .Returns<Func<Type, bool>>(f => new Type[] { typeof(FindableBootstrapperTask), typeof(object) }.Where(f));

            new Bootstrapper(resolver.Object, new[] { appdomainHelper.Object }).Run();

            resolver.Verify(r => r.RegisterType(typeof(IBootstrapperTask), typeof(FindableBootstrapperTask)), Times.Once());
            task1.Verify(t => t.Execute(), Times.Once());
        }

        /*
        [Fact]
        public void Register_Should_Use_Static_Filter()
        {
            var filtered = false;
            var appdomainHelper = new Mock<IAppDomainHelper>();
            appdomainHelper
                .Setup(h => h.GetTypes(It.IsAny<Func<Type, bool>>()))
                .Callback<Func<Type, bool>>(func => func(typeof(FindableBootstrapperTask)))
                .Returns(new Type[0]);
            Bootstrapper.TaskFilters.Add(type => filtered = true);

            new Bootstrapper(resolver.Object, new[] { appdomainHelper.Object }).Run();

            Assert.True(filtered);
        }
        */
    }

    public class FindableBootstrapperTask: IBootstrapperTask
    {
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}