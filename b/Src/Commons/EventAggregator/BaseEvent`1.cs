using System;
using System.Linq;
using BoC.EventAggregator;

namespace BoC.EventAggregator
{
    public abstract class BaseEvent<TPayload> : BaseEvent
    {
        public virtual SubscriptionToken Subscribe(Action<TPayload> action)
        {
            return Subscribe(action, false);
        }

        public virtual SubscriptionToken Subscribe(Action<TPayload> action, bool keepSubscriberReferenceAlive)
        {
            return Subscribe(action, keepSubscriberReferenceAlive, delegate { return true; });
        }

        public virtual SubscriptionToken Subscribe(Action<TPayload> action, bool keepSubscriberReferenceAlive, Predicate<TPayload> filter)
        {
            IDelegateReference actionReference = new DelegateReference(action, keepSubscriberReferenceAlive);
            IDelegateReference filterReference = new DelegateReference(filter, keepSubscriberReferenceAlive);

            EventSubscription<TPayload> subscription = new EventSubscription<TPayload>(actionReference, filterReference);

            return Subscribe(subscription);
        }

        public virtual void Publish(TPayload payload)
        {
            base.Publish(payload);
        }

        public virtual void Unsubscribe(Action<TPayload> subscriber)
        {
            lock (Subscriptions)
            {
                IEventSubscription eventSubscription = Subscriptions.Cast<EventSubscription<TPayload>>().FirstOrDefault(evt => evt.Action == subscriber);

                if (eventSubscription != null)
                {
                    Subscriptions.Remove(eventSubscription);
                }
            }
        }

        public virtual bool Contains(Action<TPayload> subscriber)
        {
            IEventSubscription eventSubscription;

            lock (Subscriptions)
            {
                eventSubscription = Subscriptions.Cast<EventSubscription<TPayload>>().FirstOrDefault(evt => evt.Action == subscriber);
            }

            return (eventSubscription != null);
        }
    }
}