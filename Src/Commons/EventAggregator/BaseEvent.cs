using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BoC.Profiling;

namespace BoC.EventAggregator
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseEvent
    {
        /// <summary>
        /// The _subscriptions
        /// </summary>
        private readonly List<IEventSubscription> _subscriptions = new List<IEventSubscription>();

        /// <summary>
        /// Gets the subscriptions.
        /// </summary>
        /// <value>
        /// The subscriptions.
        /// </value>
        protected ICollection<IEventSubscription> Subscriptions
        {
            [DebuggerStepThrough]
            get
            {
                return _subscriptions;
            }
        }

        /// <summary>
        /// Subscribes the specified event subscription.
        /// </summary>
        /// <param name="eventSubscription">The event subscription.</param>
        /// <returns></returns>
        protected virtual SubscriptionToken Subscribe(IEventSubscription eventSubscription)
        {
            eventSubscription.SubscriptionToken = new SubscriptionToken();

            lock (_subscriptions)
            {
                _subscriptions.Add(eventSubscription);
            }

            return eventSubscription.SubscriptionToken;
        }

        /// <summary>
        /// Publishes the specified arguments.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public virtual void Publish(params object[] arguments)
        {
            using (Profiler.StartContext("{0}.Publish()", this.GetType()))
            {
                List<Action<object[]>> executionStrategies = PruneAndReturnStrategies();

                foreach (var executionStrategy in executionStrategies)
                {
                    executionStrategy(arguments);
                }
            }
        }

        /// <summary>
        /// Unsubscribes the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        public virtual void Unsubscribe(SubscriptionToken token)
        {
            using (Profiler.StartContext("{0}.Unsubscribe()", this.GetType()))
            {
                lock (_subscriptions)
                {
                    IEventSubscription subscription =
                        _subscriptions.FirstOrDefault(evt => evt.SubscriptionToken == token);

                    if (subscription != null)
                    {
                        _subscriptions.Remove(subscription);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether [contains] [the specified token].
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public virtual bool Contains(SubscriptionToken token)
        {
            using (Profiler.StartContext("{0}.Contains()", this.GetType()))
            {
                lock (_subscriptions)
                {
                    IEventSubscription subscription =
                        _subscriptions.FirstOrDefault(evt => evt.SubscriptionToken == token);

                    return (subscription != null);
                }
            }
        }

        /// <summary>
        /// Prunes the and return strategies.
        /// </summary>
        /// <returns></returns>
        private List<Action<object[]>> PruneAndReturnStrategies()
        {
            List<Action<object[]>> returnList = new List<Action<object[]>>();

            lock (_subscriptions)
            {
                for (int i = _subscriptions.Count - 1; i >= 0; i--)
                {
                    Action<object[]> subscriptionAction = _subscriptions[i].GetExecutionStrategy();

                    if (subscriptionAction == null)
                    {
                        _subscriptions.RemoveAt(i);
                    }
                    else
                    {
                        returnList.Add(subscriptionAction);
                    }
                }
            }

            return returnList;
        }
    }
}