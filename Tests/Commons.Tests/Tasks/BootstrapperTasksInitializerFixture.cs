using System;
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

            new BootstrapperTasksInitializer(resolver.Object).Run();

            task1.Verify();
        }

        [Fact]
        public void Execute_Should_Register_Tasks()
        {
            var task1 = new Mock<IBootstrapperTask>();

            resolver.Setup(r => r.ResolveAll<IBootstrapperTask>()).Returns(new[] { task1.Object });
            resolver.Setup(r => r.RegisterType(typeof(IBootstrapperTask), typeof(FindableBootstrapperTask))).Verifiable();

            new BootstrapperTasksInitializer(resolver.Object).Execute();

            resolver.Verify(r => r.RegisterType(typeof(IBootstrapperTask), typeof(FindableBootstrapperTask)), Times.Once());
        }


        [Fact]
        public void Register_Should_Not_Find_And_Register_Anything_When_Filtered()
        {
            resolver.Setup(r => r.RegisterType(typeof(IBootstrapperTask), typeof(FindableBootstrapperTask))).Verifiable();

            BootstrapperTasksInitializer.TaskFilter = type => false;
            new BootstrapperTasksInitializer(resolver.Object).RegisterAllTasks();

            resolver.Verify(r => r.RegisterType(typeof(IBootstrapperTask), typeof(FindableBootstrapperTask)), Times.Never());
        }

    }

    public class FindableBootstrapperTask: IBootstrapperTask
    {
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}