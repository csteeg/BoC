using System.Web;

namespace BoC.Web
{
    public class WebRequestEventArgs
    {
        public WebRequestEventArgs(HttpRequest webRequest)
        {
            WebRequest = webRequest;
        }
        public HttpRequest WebRequest { get; set; }
    }
}