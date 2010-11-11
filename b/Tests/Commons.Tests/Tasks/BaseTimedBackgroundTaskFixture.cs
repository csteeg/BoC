using System.Threading;
using BoC.EventAggregator;
using BoC.Tasks;
using Moq;
using Moq.Protected;
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
        public void If_Not_Autoreset_DoWork_Cannot_Be_Accessed_Simultaneously()
        {
            _task.Interval = 100;
            _task.Start();
            Thread.Sleep(300);
            
            Assert.Equal(1, _task.RunCounter);
        }

        [Fact]
        public void If_Autoreset_DoWork_Can_Be_Accessed_Simultaneously()
        {
            _task.AutoReset = true;
            _task.Interval = 100;
            _task.Start();
            Thread.Sleep(1000);

            Assert.True(_task.RunCounter > 1);
        }

        [Fact]
        public void DoWork_Should_Be_In_Different_Thread()
        {
            _task.SetStopping(true);
            _task.Start();
            _task.Stop();
            Assert.NotEqual(Thread.CurrentThread.ManagedThreadId, _task.ThreadId);
        }
        
        [Fact]
        public void Stop_Should_Call_OnStop()
        {
            var task = new Mock<BaseTimedBackgroundTask>(new Mock<IEventAggregator>().Object);
            task.CallBase = true;

            task.Protected().Setup("OnStop");

            task.Object.Stop();

            task.Protected().Verify("OnStop", Times.Once());
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
                    RunCounter++;
                    ThreadId = Thread.CurrentThread.ManagedThreadId;
                    Thread.Sleep(300);
                }
            }

            public int RunCounter { get; set; }

            public int ThreadId { get; set; }

            public void SetStopping(bool value)
            {
                IsStopping = value;
            }
        }

    }
}