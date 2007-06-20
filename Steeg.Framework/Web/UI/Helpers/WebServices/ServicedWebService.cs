using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace Steeg.Framework.Web.UI.WebServices
{
    [WebService(Namespace = "http://steeg-framework.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ServicedWebService<T> : System.Web.Services.WebService where T : class
    {
        private T service;

        public ServicedWebService()
            : base()
        {
            this.service = (T)SteegWindsorContainer.Obtain()[typeof(T)];
        }

        public T Service
        {
            get { return this.service; }
        }

        public S GetFacility<S>() where S : class
        {
            return (S)SteegWindsorContainer.Obtain()[typeof(S)];
        }

    }
}