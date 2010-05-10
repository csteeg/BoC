using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace BoC.Web
{
    public class CookieAwareWebClient : WebClient
    {

        private CookieContainer m_container = new CookieContainer();
        public CookieContainer CookieContainer
        {
            get { return m_container; }
        }

        private string lastRequest;
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = m_container;
                if (!String.IsNullOrEmpty(lastRequest))
                {
                    (request as HttpWebRequest).Referer = lastRequest;
                }
                if (request.Method == "POST" && String.IsNullOrEmpty(request.ContentType))
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                }
            }

            lastRequest = address.ToString();
            return request;
        }
    }
}
