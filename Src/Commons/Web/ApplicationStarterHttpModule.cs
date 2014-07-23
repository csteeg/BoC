using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using Microsoft.Web.Infrastructure;

namespace BoC.Web
{
    public class ApplicationStarterHttpModule : IHttpModule
	{
        private static object lockObject = new object();
		private static bool startWasCalled = false;

        public static volatile bool Disabled = false;

		public static void StartApplication()
		{
			if (Disabled || startWasCalled || Initializer.Executed)
				return;
			try
			{
				lock (lockObject)
				{
				    var disabled = WebConfigurationManager.AppSettings["BoC.Web.DisableAutoStart"];
				    if ("true".Equals(disabled, StringComparison.InvariantCultureIgnoreCase))
				    {
				        Disabled = true;
				        return;
				    }

					if (startWasCalled)
						return;

					Initializer.Execute();
					startWasCalled = true;
				}
			}
			catch
			{
				InfrastructureHelper.UnloadAppDomain();
				throw;
			}
		}

		public void Init(HttpApplication context)
		{
		    if (Disabled)
		        return;

            context.BeginRequest += (sender, args) => StartApplication();
            if (!Disabled && !startWasCalled)
			{
				try
				{
					context.Application.Lock();
					StartApplication();
				}
				finally
				{
					context.Application.UnLock();
				}
			}
		}

		public void Dispose()
		{
		}
	}
}
