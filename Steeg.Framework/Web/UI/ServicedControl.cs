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
    public class ServicedUserControl<T> : System.Web.UI.UserControl where T : class
    {
        private T service;

        public ServicedUserControl()
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