using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BoC.InversionOfControl;
using BoC.InversionOfControl.Unity;
using BoC.Persistence;
using BoC.Persistence.NHibernate;
using BoC.Tasks;
using BoC.Web.Mvc;
using MvcTodo.Models.Entity;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace MvcTodo
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );

        }

        private static bool initialized = false;
        protected void Application_BeginRequest()
        {
            if (!initialized)
                Application_Start();
        }

        protected void Application_Start()
        {
            if (!IoC.IsInitialized())
            {
                try
                {
                    //     RegisterRoutes(RouteTable.Routes);
                    IoC.InitializeWith(new ExtendedUnityContainer());
                    Bootstrapper.RegisterAllTasksAndRunThem(type => true);
                    initialized = true;

                    Configuration config =
                        IoC.Resolve(typeof(Configuration)) as Configuration;

                    IRepository<Catalog> _repository = IoC.Resolve(typeof(IRepository<Catalog>)) as IRepository<Catalog>;

                    try
                    {
                        int count = _repository.ToList().Count;

                    }
                    catch (Exception e)
                    {
                        new SchemaExport(config).Create(true, true);
                        _repository.SaveOrUpdate(new Catalog { Name = "home", Description = "at home" });
                        _repository.SaveOrUpdate(new Catalog { Name = "work", Description = "at work" });

                    }
                }
                catch
                {
                    IoC.Reset();
                    throw;
                }
            }
        }

        protected void Application_EndRequest()
        {
            AutoContextSessionManager.CleanUp(IoC.Resolve<ISessionFactory>());
        }
    }
}