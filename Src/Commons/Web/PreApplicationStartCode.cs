using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using BoC.EventAggregator;
using BoC.InversionOfControl;
using BoC.Web;
using BoC.Web.Events;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(PreApplicationStartCode), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(PreApplicationStartCode), "Shutdown")]
namespace BoC.Web
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class PreApplicationStartCode
    {
        public static IList<Type> RegisterModules = new List<Type>(
            new Type[]
                {
                    typeof(DataContextPerRequestHttpModule), 
                    typeof(ApplicationStarterHttpModule), 
                    typeof(EventTriggeringHttpModule)
                });

        public static bool StartWasCalled { get; set; }

        public static void Start()
        {
            if (StartWasCalled) return;

            StartWasCalled = true;

            foreach (var module in RegisterModules)
            {
                DynamicModuleUtility.RegisterModule(module);
            }
        }

        public static void Shutdown()
        {
            PublishEvent<WebApplicationEndEvent>();
            if (IoC.Resolver != null)
                IoC.Resolver.Dispose();
        }
        private static void PublishEvent<T>() where T : BaseEvent<EventArgs>, new()
        {
            if (!IoC.IsInitialized())
                return;
            var eventAggregator = IoC.Resolver.Resolve<IEventAggregator>();
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<T>().Publish(new WebRequestEventArgs(new HttpContextWrapper(HttpContext.Current)));
            }

        }

    }
}