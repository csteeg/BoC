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
    public class AutoContextSessionManager : ISessionManager, IDisposable
    {
        public AutoContextSessionManager(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
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
                if (!AutoContextSessionContext.HasBind(SessionFactory))
                {
                    AutoContextSessionContext.Bind(SessionFactory.OpenSession());
                }
                return sessionFactory.GetCurrentSession();
            }
        }

        public static void CleanUp(ISessionFactory sessionFactory)
        {
            ISession session = AutoContextSessionContext.Unbind(sessionFactory);
            if (session != null)
            {
                if (session.Transaction != null &&
                    session.Transaction.IsActive)
                {
                    session.Transaction.Rollback();
                }
                else if (HttpContext.Current != null && HttpContext.Current.Error == null && session.IsDirty())
                {
                    session.Flush();
                }
                session.Close();
            }

        }

        public void Dispose()
        {
            if (HttpContext.Current != null)
                CleanUp(SessionFactory);
        }

    }
}