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
    public class CurrentContextSessionManager : ISessionManager, IDisposable
    {
        public CurrentContextSessionManager(ISessionFactory sessionFactory)
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
                if (!CurrentSessionContext.HasBind(SessionFactory))
                {
                    CurrentSessionContext.Bind(SessionFactory.OpenSession());
                    var sess = sessionFactory.GetCurrentSession();
                    if (sess != null)
                        sess.FlushMode = FlushMode.Commit;
                    return sess;
                }
                
                return sessionFactory.GetCurrentSession();
            }
        }

        public static void CleanUp(ISessionFactory sessionFactory)
        {
            ISession session = CurrentSessionContext.Unbind(sessionFactory);
            if (session != null)
            {
                if (session.Transaction != null &&
                    session.Transaction.IsActive)
                {
                    session.Transaction.Rollback();
                }
                else
                {
                    session.Flush();
                }
                session.Close();
            }
        }

        public void Dispose()
        {
            CleanUp(IoC.Resolve<ISessionFactory>());
        }

    }
}