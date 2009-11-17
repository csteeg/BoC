using BoC.EventAggregator;

namespace BoC.Web
{
    public class WebSessionStartEvent : BaseEvent<WebSessionEventArgs> { }
    public class WebSessionStopEvent : BaseEvent<WebSessionEventArgs> { }
    public class WebRequestBeginEvent : BaseEvent<WebRequestEventArgs> { }
    public class WebRequestEndEvent : BaseEvent<WebRequestEventArgs> { }
}