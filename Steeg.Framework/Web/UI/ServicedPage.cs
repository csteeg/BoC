using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Steeg.Framework.Web.UI
{
    public class ServicedPage<T> : System.Web.UI.Page where T : class
    {
        private T service;

        public ServicedPage()
            : base()
        {
            this.service = (T)SteegWindsorContainer.Obtain().Resolve<T>();
        }

        public T Service
        {
            get { return this.service; }
        }

        public TC Get<TC>() where TC : class
        {
            return SteegWindsorContainer.Obtain().Resolve<TC>();
        }

    }
}