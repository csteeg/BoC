using System;
using System.Linq;
using BoC.Helpers;
using BoC.Tasks;
using Moq;
using Xunit;

namespace BoC.Tests.Tasks
{
    public class RegisterBackgroundTasksFixture : BaseIoCFixture
    {
        [Fact]
        public void Execute_Should_Register_Tasks()
        {
            var task2 = new Mock<IBackgroundTask>();
            var appdomainHelper = new Mock<IAppDomainHelper>();
            appdomainHelper.Setup(h => h.GetTypes(It.IsAny<Func<Type, bool>>()))
                .Returns<Func<Type, bool>>(f => new Type[] { typeof(FindableBackgroundTask), typeof(object) }.Where(f));

            new RegisterBackgroundTasks(resolver.Object, new[] { appdomainHelper.Object }).Execute();

            resolver.Verify(r => r.RegisterType(typeof(IBackgroundTask), typeof(FindableBackgroundTask)), Times.Once());
        }

        [Fact]
        public void Register_Should_Use_StaticFilter()
        {
            var filtered = false;
            var appdomainHelper = new Mock<IAppDomainHelper>();

            appdomainHelper
                .Setup(h => h.GetTypes(It.IsAny<Func<Type, bool>>()))
                .Callback<Func<Type, bool>>(func => func(typeof (FindableBackgroundTask)))
                .Returns(new Type[0]);

            RegisterBackgroundTasks.TaskFilter = type => filtered = true;
            new RegisterBackgroundTasks(resolver.Object, new[] {appdomainHelper.Object}).Execute();

            Assert.True(filtered);
        }

    }

    public class FindableBackgroundTask: IBackgroundTask
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool IsRunning
        {
            get { throw new NotImplementedException(); }
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}