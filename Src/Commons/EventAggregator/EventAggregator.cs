using BoC.Profiling;

namespace BoC.EventAggregator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        /// <summary>
        /// The _events
        /// </summary>
        private readonly List<BaseEvent> _events = new List<BaseEvent>();
        /// <summary>
        /// The _RWL
        /// </summary>
        private readonly ReaderWriterLockSlim _rwl = new ReaderWriterLockSlim();

        /// <summary>
        /// Gets the event.
        /// </summary>
        /// <typeparam name="TEventType">The type of the event type.</typeparam>
        /// <returns></returns>
        public TEventType GetEvent<TEventType>() where TEventType : BaseEvent, new()
        {
            return GetEvent(typeof (TEventType)) as TEventType;
        }

        /// <summary>
        /// Gets the event.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns></returns>
        public BaseEvent GetEvent(Type eventType)
        {
            using (Profiler.StartContext("EventAggregator.GetEvent({0})", eventType))
            {
                _rwl.EnterUpgradeableReadLock();

                try
                {
                    var eventInstance = _events.SingleOrDefault(evt => evt.GetType() == eventType);

                    if (eventInstance == null)
                    {
                        _rwl.EnterWriteLock();

                        try
                        {
                            eventInstance = _events.SingleOrDefault(evt => evt.GetType() == eventType);

                            if (eventInstance == null)
                            {
                                eventInstance = Activator.CreateInstance(eventType) as BaseEvent;
                                _events.Add(eventInstance);
                            }
                        }
                        finally
                        {
                            _rwl.ExitWriteLock();
                        }
                    }

                    return eventInstance;
                }
                finally
                {
                    _rwl.ExitUpgradeableReadLock();
                }
            }
        }
    }
}