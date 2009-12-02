using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using System.Web;
using BoC.UnitOfWork;
using NHibernate;

namespace BoC.Persistence.NHibernate.UnitOfWork
{
    public class NHibernateUnitOfWork : BaseThreadSafeSingleUnitOfWork
    {
        private readonly ISessionFactory sessionFactory;
        private ISession session;

        public NHibernateUnitOfWork(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        override protected void CleanUp()
        {
            if (session != null)
            {
                try
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
                }
                finally
                {
                    session = null;
                }
            }
        }

        public ISession Session
        {
            get
            {
                if (OuterUnitOfWork == this)
                {
                    if (this.session == null)
                    {
                        this.session = sessionFactory.OpenSession();
                    }
                    return session;
                }
                else
                {
                    return ((NHibernateUnitOfWork)OuterUnitOfWork).Session;
                }
            }
        }


    }
}
