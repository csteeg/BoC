using System.Web;
using Microsoft.Web.Infrastructure;

namespace BoC.Web
{
    public class ApplicationStarterHttpModule : IHttpModule
	{
        private static object lockObject = new object();
		private static bool startWasCalled = false;

		public static void StartApplication()
		{
			if (startWasCalled || Initializer.Executed)
				return;

			try
			{
				lock (lockObject)
				{
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
            context.BeginRequest += (sender, args) => StartApplication();
            if (!startWasCalled)
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
