using System;
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

            resolver.Setup(r => r.RegisterType(typeof(IBackgroundTask), typeof(FindableBackgroundTask))).Verifiable();

            new RegisterBackgroundTasks(resolver.Object).Execute();
            
            task2.Verify();
            resolver.Verify();
        }

        [Fact]
        public void Register_Should_Not_Find_And_Register_Anything_When_Filtered()
        {
            resolver.Setup(r => r.RegisterType(typeof(IBackgroundTask), typeof(FindableBackgroundTask))).Verifiable();

            RegisterBackgroundTasks.TaskFilter = type => false;
            new RegisterBackgroundTasks(resolver.Object).Execute();

            resolver.Verify(r => r.RegisterType(typeof(IBackgroundTask), typeof(FindableBackgroundTask)), Times.Never());
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