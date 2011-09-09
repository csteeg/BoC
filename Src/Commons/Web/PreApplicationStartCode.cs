using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using BoC.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: PreApplicationStartMethod(typeof(PreApplicationStartCode), "Start")]
namespace BoC.Web
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class PreApplicationStartCode
    {
        public static IList<Type> RegisterModules = new List<Type>(
            new Type[]
                {
                    typeof(UnitOfWorkPerRequestHttpModule), 
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
    }
}