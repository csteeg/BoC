using System;
using BoC.EventAggregator;
using BoC.Tasks;
using Moq;
using Moq.Protected;
using Xunit;

namespace BoC.Tests.Tasks
{
    public class BaseBackgroundTaskFixture
    {
        private readonly Mock<IEventAggregator> _eventAggregator;
        private readonly Mock<TestableBackgroundTask> _task;

        public BaseBackgroundTaskFixture()
        {
            _eventAggregator = new Mock<IEventAggregator>();

            _task = new Mock<TestableBackgroundTask>(_eventAggregator.Object);
        }

        [Fact]
        public void IsRunning_Should_Be_False_When_New_Instance_Is_Created()
        {
            Assert.False(_task.Object.IsRunning);
        }

        [Fact]
        public void Dispose_Should_Call_Stop()
        {
            using (_task.Object)
            {
                _task.Object.Start();
            }

            Assert.False(_task.Object.IsRunning);
        }

        [Fact]
        public void Start_Should_Change_Is_Running_Status()
        {
            _task.Object.Stop();
            _task.Object.Start();

            Assert.True(_task.Object.IsRunning);
        }

        [Fact]
        public void Stop_Should_Change_Is_Running_Status()
        {
            _task.Object.Start();
            _task.Object.Stop();

            Assert.False(_task.Object.IsRunning);
        }
        
        [Fact]
        public void Start_Should_Call_OnStart()
        {
            _task.Protected().Setup("OnStart");
            
            _task.Object.Start();

            _task.Protected().Verify("OnStart", Times.Once());
        }

        [Fact]
        public void Stop_Should_Call_OnStop()
        {
            _task.Protected().Setup("OnStop");

            _task.Object.Stop();

            _task.Protected().Verify("OnStop", Times.Once());
        }

        [Fact]
        public void Subscribe_Should_Use_EventAggregator()
        {
            var userActivateEvent = new Mock<DummyEvent>();

            _eventAggregator.Setup(ea => ea.GetEvent<DummyEvent>()).Returns(userActivateEvent.Object).Verifiable();
            userActivateEvent.Setup(e => e.Subscribe(It.IsAny<Action<DummyEventArgs>>(), true)).Returns(new SubscriptionToken()).Verifiable();

            _task.Object.CallSubscribe<DummyEvent, DummyEventArgs>(delegate { });

            _eventAggregator.Verify();
            userActivateEvent.Verify();
        }

        [Fact]
        public void Unsubscribe_Should_Use_EventAggregator()
        {
            var userActivateEvent = new Mock<DummyEvent>();

            _eventAggregator.Setup(ea => ea.GetEvent<DummyEvent>()).Returns(userActivateEvent.Object).Verifiable();
            userActivateEvent.Setup(e => e.Unsubscribe(It.IsAny<SubscriptionToken>())).Verifiable();

            _task.Object.CallUnsubscribe<DummyEvent>(new SubscriptionToken());

            _eventAggregator.Verify();
            userActivateEvent.Verify();
        }

        public class DummyEventArgs
        {
        }

        public class DummyEvent : BaseEvent<DummyEventArgs>
        {
        }

        public class TestableBackgroundTask: BaseBackgroundTask
        {
            public TestableBackgroundTask(IEventAggregator eventAggregator) : base(eventAggregator)
            {
            }

            internal SubscriptionToken CallSubscribe<TEvent, TEventArgs>(Action<TEventArgs> action)
                where TEvent : BaseEvent<TEventArgs>, new()
                where TEventArgs : class
            {
                return base.Subscribe<TEvent,TEventArgs> (action);
            }

            internal void CallUnsubscribe<TEvent>(SubscriptionToken token) where TEvent : BaseEvent, new()
            {
                base.Unsubscribe<TEvent>(token);
            }

            protected override void OnStart()
            {
                
            }

            protected override void OnStop()
            {
                
            }
        }
    }
}