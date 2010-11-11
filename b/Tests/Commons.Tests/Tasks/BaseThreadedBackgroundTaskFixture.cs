using System.Threading;
using BoC.EventAggregator;
using BoC.Tasks;
using Moq;
using Xunit;

namespace BoC.Tests.Tasks
{
    public class BaseThreadedBackgroundTaskFixture : BaseBackgroundTaskFixture
    {
        private readonly TestableThreadedBackgroundTask _task;

        public BaseThreadedBackgroundTaskFixture()
        {
            _task = new TestableThreadedBackgroundTask(new Mock<IEventAggregator>().Object);
        }

        [Fact]
        public void IsStopping_Should_Be_False_When_New_Instance_Is_Created()
        {
            Assert.False(_task.IsStopping);
        }

        [Fact]
        public void When_IsStopping_Is_True_Thread_Should_Exit_Immediately()
        {
            _task.SetStopping(true);
            _task.Start();
            
            Assert.True(_task.IsStopping);
            Assert.False(_task.IsRunning);
        }

        [Fact]
        public void DoWork_Should_Be_In_Different_Thread()
        {
            _task.SetStopping(true);
            _task.Start();

            Assert.NotEqual(Thread.CurrentThread.ManagedThreadId, _task.ThreadId);
        }

        public class TestableThreadedBackgroundTask : BaseThreadedBackgroundTask
        {
            public TestableThreadedBackgroundTask(IEventAggregator eventAggregator) : base(eventAggregator)
            {
            }

            public override void DoWork()
            {
                if (!IsStopping)
                {
                    ThreadId = Thread.CurrentThread.ManagedThreadId;
                    Thread.Sleep(300);

                    DoWork();
                }
            }

            public int ThreadId { get; set; }

            public void SetStopping(bool value)
            {
                IsStopping = value;
            }
        }

    }
}