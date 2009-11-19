using System.Web;
using BoC.EventAggregator;

namespace BoC.Web.Events
{
    public class WebSessionBeginEvent : BaseEvent<WebRequestEventArgs> { }
    public class WebSessionEndEvent : BaseEvent<SessionEndEventArgs> { }
    public class WebRequestBeginEvent : BaseEvent<WebRequestEventArgs> { }
    public class WebRequestEndEvent : BaseEvent<WebRequestEventArgs> { }
    public class WebPostAuthorizeEvent : BaseEvent<WebRequestEventArgs> { }
    public class WebAuthorizeEvent : BaseEvent<WebRequestEventArgs> { }
    public class WebAuthenticateEvent : BaseEvent<WebRequestEventArgs> { }
    public class WebPostAuthenticateEvent : BaseEvent<WebRequestEventArgs> { }
    public class WebRequestErrorEvent : BaseEvent<WebRequestEventArgs> { }
    public class WebRequestPreHandlerExecute : BaseEvent<WebRequestEventArgs> { }
    public class WebRequestPostHandlerExecute : BaseEvent<WebRequestEventArgs> { }

    public class WebRequestEventArgs
    {
        public WebRequestEventArgs(HttpContextBase context)
        {
            HttpContext = context;
        }
        public HttpContextBase HttpContext { get; set; }
    }

    public class SessionEndEventArgs
    {

        public SessionEndEventArgs(HttpSessionStateWrapper sessionState)
        {
            SessionState = sessionState;
        }
        
        public HttpSessionStateWrapper SessionState { get; set; }
    }
}