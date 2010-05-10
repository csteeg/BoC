using System.Threading;
using BoC.EventAggregator;
using BoC.Tasks;
using Moq;
using Xunit;

namespace BoC.Tests.Tasks
{
    public class BaseTimedBackgroundTaskFixture
    {
        private readonly TestableTimedBackgroundTask _task;

        public BaseTimedBackgroundTaskFixture()
        {
            _task = new TestableTimedBackgroundTask(new Mock<IEventAggregator>().Object);
        }

        [Fact]
        public void IsStopping_Should_Be_False_When_New_Instance_Is_Created()
        {
            Assert.False(_task.IsStopping);
        }

        [Fact]
        public void AutoReset_Should_Be_False_When_New_Instance_Is_Created()
        {
            Assert.False(_task.AutoReset);
        }

        [Fact]
        public void Interval_Should_Be_Five_Seconds_When_New_Instance_Is_Created()
        {
            Assert.Equal(5000, _task.Interval);
        }

        [Fact]
        public void If_Not_Autoreset_DoWork_Cannot_Be_Access_Simultaneous()
        {
            _task.Interval = 100;
            _task.Start();
        }

        [Fact]
        public void DoWork_Should_Be_In_Different_Thread()
        {
            _task.SetStopping(true);
            _task.Start();

            Assert.NotEqual(Thread.CurrentThread.ManagedThreadId, _task.ThreadId);
        }

        public class TestableTimedBackgroundTask : BaseTimedBackgroundTask
        {
            public TestableTimedBackgroundTask(IEventAggregator eventAggregator)
                : base(eventAggregator)
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