namespace BoC.EventAggregator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class EventAggregator : IEventAggregator
    {
        private readonly List<BaseEvent> _events = new List<BaseEvent>();
        private readonly ReaderWriterLockSlim _rwl = new ReaderWriterLockSlim();

        public TEventType GetEvent<TEventType>() where TEventType : BaseEvent
        {
            _rwl.EnterUpgradeableReadLock();

            try
            {
                TEventType eventInstance = _events.SingleOrDefault(evt => evt.GetType() == typeof(TEventType)) as TEventType;

                if (eventInstance == null)
                {
                    _rwl.EnterWriteLock();

                    try
                    {
                        eventInstance = _events.SingleOrDefault(evt => evt.GetType() == typeof(TEventType)) as TEventType;

                        if (eventInstance == null)
                        {
                            eventInstance = Activator.CreateInstance<TEventType>();
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