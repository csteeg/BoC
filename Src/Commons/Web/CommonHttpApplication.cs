using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using BoC.EventAggregator;
using BoC.InversionOfControl;
using BoC.Tasks;

namespace BoC.Web
{
    public abstract class CommonHttpApplication : HttpApplication
    {
        private static bool _initialized = false;

        virtual protected void Application_Start()
        {
            if (!_initialized)
            {
                _initialized = true;
                Bootstrapper.RegisterAllTasksAndRunThem(type => true);
            }

        }

        virtual protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (!_initialized)
                Application_Start();
        }

        virtual protected void Session_Start()
        {
            var eventAggregator = IoC.Resolve<IEventAggregator>();
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<WebSessionStartEvent>().Publish(new WebSessionEventArgs(HttpContext.Current.Session));
            }
        }
    }
}
