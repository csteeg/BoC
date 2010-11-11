using System;
using System.Collections.Generic;
using BoC.EventAggregator;
using Moq;
using Xunit;

namespace BoC.Tests.EventAggregator
{
    public class BaseEventFixture
    {
        private readonly BaseEventTestDouble @event;

        public BaseEventFixture()
        {
            @event = new BaseEventTestDouble();
        }

        [Fact]
        public void Subscribe_Should_Increase_Subscribtions()
        {
            int previousCount = @event.SubscriptionColection.Count;

            Subscribe();

            Assert.True(@event.SubscriptionColection.Count > previousCount);
        }

        [Fact]
        public void Contains_Should_Return_True_When_Token_Exists()
        {
            var token = Subscribe();

            Assert.True(@event.Contains(token));
        }

        [Fact]
        public void Unsubscribe_Should_Decrease_Subscribtions()
        {
            var token = Subscribe();
            int previousCount = @event.SubscriptionColection.Count;

            @event.Unsubscribe(token);

            Assert.True(@event.SubscriptionColection.Count < previousCount);
        }

        [Fact]
        public void Publish_Should_Raise_Event()
        {
            var subscription = new Mock<IEventSubscription>();

            var isRaised = false;

            subscription.Setup(s => s.GetExecutionStrategy()).Returns(delegate { isRaised = true; });
            subscription.Setup(s => s.SubscriptionToken).Returns(new SubscriptionToken());

            @event.Subscribe(subscription.Object);
            @event.Publish();

            Assert.True(isRaised);
        }

        [Fact]
        public void Publish_Should_Remove_From_Subscribtion_Is_Action_Is_Null()
        {
            var subscription = new Mock<IEventSubscription>();

            subscription.Setup(s => s.GetExecutionStrategy()).Returns((Action<object[]>)null);

            @event.Subscribe(subscription.Object);
            @event.Publish();

            Assert.True(@event.SubscriptionColection.Count == 0);
        }

        private SubscriptionToken Subscribe()
        {
            var eventSubscription = new Mock<IEventSubscription>();

            return @event.Subscribe(eventSubscription.Object);
        }
    }

    public class BaseEventTestDouble : BaseEvent
    {
        public ICollection<IEventSubscription> SubscriptionColection
        {
            get
            {
                return Subscriptions;
            }
        }

        public new SubscriptionToken Subscribe(IEventSubscription eventSubscription)
        {
            return base.Subscribe(eventSubscription);
        }

        public new void Publish(params object[] arguments)
        {
            base.Publish(arguments);
        }
    }
}