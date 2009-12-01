using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Transactions;
using System.Web;
using BoC.UnitOfWork;
using NHibernate;

namespace BoC.Persistence.NHibernate.UnitOfWork
{
    public class NHibernateUnitOfWork: IUnitOfWork
    {
        private readonly ISessionFactory sessionFactory;
        private ISession session;
        
        [ThreadStatic]
        private static NHibernateUnitOfWork outerUnitOfWork = null;
        public static NHibernateUnitOfWork OuterUnitOfWork { get { return outerUnitOfWork; } }

        public NHibernateUnitOfWork(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
            if (outerUnitOfWork == null)
            {
                outerUnitOfWork = this;
            }
        }

        public void Dispose()
        {
            if (outerUnitOfWork == this)
            {
                CleanUp();
                outerUnitOfWork = null;
            }
        }

        public void CleanUp()
        {
            if (outerUnitOfWork == this && session != null)
            {
                if (session.Transaction != null &&
                    session.Transaction.IsActive)
                {
                    session.Transaction.Rollback();
                }
                else if (Transaction.Current != null)
                {
                    Transaction.Current.Rollback();
                }
                else if (session.IsDirty())
                {
                    session.Flush();
                }
                session.Close();
                session.Dispose();
                session = null;
            }
        }

        public ISession Session
        {
            get
            {
                if (outerUnitOfWork == this)
                {
                    if (this.session == null)
                    {
                        this.session = sessionFactory.OpenSession();
                    }
                    return session;
                }
                else
                {
                    return outerUnitOfWork.Session;
                }
            }
        }

    }
}
