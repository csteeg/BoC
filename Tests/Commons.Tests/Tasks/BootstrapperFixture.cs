using System;
using BoC.Tasks;
using Moq;
using Xunit;

namespace BoC.Tests.Tasks
{
    public class BootstrapperFixture: BaseIoCFixture
    {
        [Fact]
        public void Run_Should_Execute_Tasks()
        {
            var task1 = new Mock<IBootstrapperTask>();
            var task2 = new Mock<IBackgroundTask>();
            var task3 = new Mock<IPostBootstrapperTask>();

            task1.Setup(t => t.Execute()).Verifiable();
            task2.Setup(t => t.Start()).Verifiable();
            task3.Setup(t => t.Execute()).Verifiable();

            resolver.Setup(r => r.ResolveAll<IBootstrapperTask>()).Returns(new[] { task1.Object });
            resolver.Setup(r => r.ResolveAll<IBackgroundTask>()).Returns(new[] { task2.Object });
            resolver.Setup(r => r.ResolveAll<IPostBootstrapperTask>()).Returns(new[] { task3.Object });

            BoC.Tasks.Bootstrapper.Run();

            task1.Verify();
            task2.Verify();
            task3.Verify();
        }

        [Fact]
        public void RegisterAllTasksAndRunThem_Should_Register_And_Execute_Tasks()
        {
            var task1 = new Mock<IBootstrapperTask>();
            var task2 = new Mock<IBackgroundTask>();
            var task3 = new Mock<IPostBootstrapperTask>();

            task1.Setup(t => t.Execute()).Verifiable();
            //task2.Setup(t => t.Start()).Verifiable();
            task3.Setup(t => t.Execute()).Verifiable();

            resolver.Setup(r => r.ResolveAll<IBootstrapperTask>()).Returns(new[] { task1.Object });
            resolver.Setup(r => r.ResolveAll<IBackgroundTask>()).Returns(new[] { task2.Object });
            resolver.Setup(r => r.ResolveAll<IPostBootstrapperTask>()).Returns(new[] { task3.Object });

            resolver.Setup(r => r.RegisterType(typeof(IBackgroundTask), typeof(FindableBackgroundTask))).Verifiable();
            resolver.Setup(r => r.RegisterType(typeof(IBootstrapperTask), typeof(FindableBootstrapperTask))).Verifiable();
            resolver.Setup(r => r.RegisterType(typeof(IPostBootstrapperTask), typeof(FindablePostBootstrapperTask))).Verifiable();

            BoC.Tasks.Bootstrapper.RegisterAllTasksAndRunThem(t => true);

            task1.Verify();
            //task2.Verify();
            task3.Verify();
            resolver.Verify();
        }

        [Fact]
        public void Register_Should_Find_And_Register_IBackgroundTasks()
        {
            resolver.Setup(r => r.RegisterType(typeof(IBackgroundTask), typeof(FindableBackgroundTask))).Verifiable();

            Bootstrapper.RegisterAllTasks(t => true);

            resolver.Verify(r => r.RegisterType(typeof(IBackgroundTask), typeof(FindableBackgroundTask)), Times.Once());
        }

        [Fact]
        public void Register_Should_Find_And_Register_IBootstrapperTasks()
        {
            resolver.Setup(r => r.RegisterType(typeof(IBootstrapperTask), typeof(FindableBootstrapperTask))).Verifiable();

            Bootstrapper.RegisterAllTasks(t => true);

            resolver.Verify(r => r.RegisterType(typeof(IBootstrapperTask), typeof(FindableBootstrapperTask)), Times.Once());
        }

        [Fact]
        public void Register_Should_Find_And_Register_IPostBootstrapperTasks()
        {
            resolver.Setup(r => r.RegisterType(typeof(IPostBootstrapperTask), typeof(FindablePostBootstrapperTask))).Verifiable();

            Bootstrapper.RegisterAllTasks(t => true);

            resolver.Verify(r => r.RegisterType(typeof(IPostBootstrapperTask), typeof(FindablePostBootstrapperTask)), Times.Once());
        }

        [Fact]
        public void Register_Should_Not_Find_And_Register_Anything_When_Filtered()
        {
            resolver.Setup(r => r.RegisterType(typeof(IBackgroundTask), typeof(FindableBackgroundTask))).Verifiable();
            resolver.Setup(r => r.RegisterType(typeof(IPostBootstrapperTask), typeof(FindableBootstrapperTask))).Verifiable();
            resolver.Setup(r => r.RegisterType(typeof(IBootstrapperTask), typeof(FindableBootstrapperTask))).Verifiable();

            Bootstrapper.RegisterAllTasks(t => false);

            resolver.Verify(r => r.RegisterType(typeof(IBackgroundTask), typeof(FindableBackgroundTask)), Times.Never());
            resolver.Verify(r => r.RegisterType(typeof(IPostBootstrapperTask), typeof(FindableBootstrapperTask)), Times.Never());
            resolver.Verify(r => r.RegisterType(typeof(IBootstrapperTask), typeof(FindableBootstrapperTask)), Times.Never());
        }

    }

    public class FindablePostBootstrapperTask : IPostBootstrapperTask {
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
    public class FindableBootstrapperTask: IBootstrapperTask
    {
        public void Execute()
        {
            throw new NotImplementedException();
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