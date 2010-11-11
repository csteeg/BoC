namespace BoC.EventAggregator
{
    using System;

    public interface IEventSubscription
    {
        SubscriptionToken SubscriptionToken
        {
            get;
            set;
        }

        Action<object[]> GetExecutionStrategy();
    }
}