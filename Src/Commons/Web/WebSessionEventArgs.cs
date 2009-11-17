using System.Web.SessionState;

namespace BoC.Web
{
    public class WebSessionEventArgs
    {
        public WebSessionEventArgs(HttpSessionState sessionState)
        {
            SessionState = sessionState;
        }
        
        public HttpSessionState SessionState { get; set; }
    }
}