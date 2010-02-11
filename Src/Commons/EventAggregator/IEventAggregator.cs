using System;

namespace BoC.EventAggregator
{
    public interface IEventAggregator
    {
        TEventType GetEvent<TEventType>() where TEventType : BaseEvent, new();
        BaseEvent GetEvent(Type eventType);
    }
}