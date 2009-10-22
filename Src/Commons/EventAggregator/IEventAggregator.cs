namespace BoC.EventAggregator
{
    public interface IEventAggregator
    {
        TEventType GetEvent<TEventType>() where TEventType : BaseEvent;
    }
}