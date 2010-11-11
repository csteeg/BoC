using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.Routing;
using BoC.InversionOfControl;
using BoC.InversionOfControl.Unity;
using BoC.Persistence.NHibernate;
using BoC.Tasks;
using NHibernate;

namespace NorthwindMvcScaffold
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            if (!IoC.IsInitialized())
            {
                try
                {
                    IoC.InitializeWith(new ExtendedUnityContainer());
                    Bootstrapper.RegisterAllTasksAndRunThem(type => true);
                }
                catch
                {
                    AppDomain.Unload(AppDomain.CurrentDomain);
                    throw;
                }
            }
        }

        protected void Application_EndRequest()
        {
            WebSessionManager.CleanUp(HttpContext.Current, IoC.Resolve<ISessionFactory>());
        }
    }
}