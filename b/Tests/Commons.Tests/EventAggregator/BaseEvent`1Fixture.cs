using System;
using System.Collections.Generic;
using BoC.EventAggregator;
using Xunit;

namespace BoC.Tests.EventAggregator
{
    public class BaseEventGenericFixture
    {
        private readonly BaseEventTestDouble<string> @event;

        public BaseEventGenericFixture()
        {
            @event = new BaseEventTestDouble<string>();
        }

        [Fact]
        public void Subscribe_With_ActionShould_Return_New_Token()
        {
            var token = @event.Subscribe(delegate { });

            Assert.NotNull(token);
        }

        [Fact]
        public void Subscribe_With_Action_And_KeepAlive_Should_Return_New_Token()
        {
            var token = @event.Subscribe(delegate { }, true);

            Assert.NotNull(token);
        }

        [Fact]
        public void Subscribe_With_Action_KeepAlive_And_Filter_Should_Return_New_Token()
        {
            var token = @event.Subscribe(delegate { }, true, delegate { return true; });

            Assert.NotNull(token);
        }

        [Fact]
        public void Publish_Should_Raise_Event()
        {
            bool isFired = false;

            @event.Subscribe(delegate { isFired = true; });

            @event.Publish("fireIt");

            Assert.True(isFired);
        }

        [Fact]
        public void Unsubscribe_Should_Decrease_Subscriptions()
        {
            Action<string> fireIt = aParam => Console.Write(aParam);

            @event.Subscribe(fireIt);

            var previousCount = @event.SubscriptionCollection.Count;

            @event.Unsubscribe(fireIt);

            Assert.True(@event.SubscriptionCollection.Count < previousCount);
        }

        [Fact]
        public void Contains_Should_Return_True_When_Subscriber_Exists()
        {
            Action<string> fireIt = aParam => Console.Write(aParam);

            @event.Subscribe(fireIt);

            Assert.True(@event.Contains(fireIt));
        }
    }

    public class BaseEventTestDouble<TPayload> : BaseEvent<TPayload>
    {
        public ICollection<IEventSubscription> SubscriptionCollection
        {
            get
            {
                return Subscriptions;
            }
        }
    }
}