using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using BoC.InversionOfControl;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;

namespace BoC.Persistence.NHibernate
{
    public class WebSessionManager : ISessionManager, IDisposable
    {
        public WebSessionManager(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
            if (HttpContext.Current != null && HttpContext.Current.ApplicationInstance != null)
            {
                HttpContext.Current.ApplicationInstance.EndRequest += (sender, args) => CleanUp(HttpContext.Current);
            }
        }

        private readonly ISessionFactory sessionFactory;
        public ISessionFactory SessionFactory 
        {
            get
            {
                return sessionFactory;
            }
        }

        public ISession Session
        {
            get
            {
                if (!ManagedWebSessionContext.HasBind(HttpContext.Current, SessionFactory))
                {
                    ManagedWebSessionContext.Bind(HttpContext.Current, SessionFactory.OpenSession());
                }
                return sessionFactory.GetCurrentSession();
            }
        }

        public static void CleanUp(HttpContext context, ISessionFactory sessionFactory)
        {
            ISession session = ManagedWebSessionContext.Unbind(context, sessionFactory);
            if (session != null)
            {
                if (session.Transaction != null &&
                    session.Transaction.IsActive)
                {
                    session.Transaction.Rollback();
                }
                else if (context.Error == null && session.IsDirty())
                {
                    session.Flush();
                }
                session.Close();
            }
            
        }

        public void CleanUp(HttpContext context)
        {
            CleanUp(context, sessionFactory);
        }

        public void Dispose()
        {
            if (HttpContext.Current != null)
                CleanUp(HttpContext.Current);
        }

    }
}