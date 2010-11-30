using System;
using System.ComponentModel;
using System.Net.Mime;
using System.Web;
using BoC.EventAggregator;
using BoC.InversionOfControl;
using BoC.UnitOfWork;
using BoC.Web;
using BoC.Web.Events;
using Microsoft.Web.Infrastructure;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: PreApplicationStartMethod(typeof(PreApplicationStartCode), "Start")]
namespace BoC.Web
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class PreApplicationStartCode
	{
		private static bool _startWasCalled;

		public static void Start()
		{
			if (_startWasCalled) return;

			_startWasCalled = true;

			DynamicModuleUtility.RegisterModule(typeof(EventTriggeringHttpModule));
		}
	}

	public class EventTriggeringHttpModule : IHttpModule
	{
		private static string unitofworkkey = "BoC.Web.CommonHttpApplication.OuterUnitOfWork";
		private static object lockObject = new object();
		private static bool startWasCalled = false;

		public static void StartApplication(HttpApplication context)
		{
			if (startWasCalled)
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

			PublishEvent<WebApplicationStartEvent, WebApplicationEventArgs>(() => new WebApplicationEventArgs(context));
		}

		private static void PublishEvent<T, TEventArgs>(Func<TEventArgs> args) where T: BaseEvent<TEventArgs>, new()
		{
			var eventAggregator = IoC.Resolver.Resolve<IEventAggregator>();
			if (eventAggregator != null)
			{
				eventAggregator.GetEvent<T>().Publish(args());
			}

		}

		private static void PublishEvent<T>() where T : BaseEvent<WebRequestEventArgs>, new()
		{
			var eventAggregator = IoC.Resolver.Resolve<IEventAggregator>();
			if (eventAggregator != null)
			{
				eventAggregator.GetEvent<T>().Publish(new WebRequestEventArgs(new HttpContextWrapper(HttpContext.Current)));
			}

		}

		public void Init(HttpApplication context)
		{
			if (!startWasCalled)
			{
				try
				{
					context.Application.Lock();
					StartApplication(context);
				}
				finally
				{
					context.Application.UnLock();
				}
			}

			context.BeginRequest += (sender, args) =>
			{
				((HttpApplication)sender).Context.Items[unitofworkkey] = UnitOfWork.UnitOfWork.BeginUnitOfWork();
				PublishEvent<WebRequestBeginEvent>();
			};
			context.EndRequest += (sender, args) =>
			{
				PublishEvent<WebRequestEndEvent>();

				var unitOfWork = ((HttpApplication)sender).Context.Items[unitofworkkey] as IUnitOfWork;
				if (unitOfWork != null)
				{
					unitOfWork.Dispose();
				}
				((HttpApplication)sender).Context.Items.Remove(unitofworkkey);
			};
			context.PostAuthorizeRequest += (sender, args) => PublishEvent<WebPostAuthorizeEvent>();
			context.AuthorizeRequest += (sender, args) => PublishEvent<WebPostAuthorizeEvent>();
			context.AuthenticateRequest += (sender, args) => PublishEvent<WebAuthenticateEvent>();
			context.PostAuthenticateRequest += (sender, args) => PublishEvent<WebPostAuthenticateEvent>();
			context.Error += (sender, args) => PublishEvent<WebRequestErrorEvent>();
			context.PreRequestHandlerExecute += (sender, args) => PublishEvent<WebRequestPreHandlerExecute>();
			context.PostRequestHandlerExecute += (sender, args) => PublishEvent<WebRequestPostHandlerExecute>();
		}

		public void Dispose()
		{
		}
	}
}
