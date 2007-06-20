using Castle.Core.Logging;
using Castle.Windsor;
using Castle.Facilities.NHibernateIntegration;
using NHibernate;

using System;
using System.Web;


namespace Steeg.Framework.Web
{
    public class SteegApplicationModule : IHttpModule, IDisposable, IContainerAccessor
    {
        protected static readonly String SessionKey = "Steeg.Framework.SessionWebModule.session";
        private static string containerLock = String.Empty;
        private static WindsorContainer container;
        
        public void Init(HttpApplication app)
        {
            InitContainer();

            app.BeginRequest += new EventHandler(OnBeginRequest);
            app.EndRequest += new EventHandler(OnEndRequest);
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            ISessionManager sessManager = (ISessionManager)Container[typeof(ISessionManager)];

            HttpContext.Current.Items.Add(SessionKey, sessManager.OpenSession());
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            ISession session = (ISession)HttpContext.Current.Items[SessionKey];

            if (session != null)
            {
                //The only difference with castle's sessionwebmodule:
                //cause the session to flush if necessary
                if ((session.Transaction == null) && session.IsDirty())
                    session.Flush();

                session.Dispose();
            }
        }

        public void Dispose()
        {
            lock (containerLock)
            {
                if (container != null)
                    container.Dispose();
            }
        }

        static public void InitContainer()
        {
            lock (containerLock)
            {
                if (container == null)
                {
                    container = new SteegWindsorContainer();
                }
            }
            
            container.Resolve<ILogger>().Info("Container initialized");
        }

        public void Application_OnStart()
        {
            InitContainer();
        }

        public I GetFacility<I>() where I : class
        {
            return (I)this.Container[typeof(I)];
        }

        #region IContainerAccessor Members

        public Castle.Windsor.IWindsorContainer Container
        {
            get 
            {
                return GetContainer(); 
            }
        }
        #endregion

        public static Castle.Windsor.IWindsorContainer GetContainer()
        {
            if (container == null)
                InitContainer();
            return container;
        }
   }
}