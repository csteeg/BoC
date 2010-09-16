using BoC.UnitOfWork;
using DevExpress.Xpo;

namespace BoC.Persistence.Xpo.UnitOfWork
{
    public class XpoUnitOfWork : BaseThreadSafeSingleUnitOfWork
    {
        private readonly ISessionFactory sessionFactory;
        private Session session;

        public XpoUnitOfWork(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        override protected void CleanUp()
        {
            if (session != null)
            {
                try
                {
                    if (session.InTransaction)
                    {
                        session.RollbackTransaction();
                    }
                    session.Dispose();
                }
                finally
                {
                    session = null;
                }
            }
        }

        public Session Session
        {
            get
            {
                if (OuterUnitOfWork == this)
                {
                    if (session == null)
                    {
                        session = sessionFactory.OpenSession();
                    }
                    return session;
                }
                else if (OuterUnitOfWork != null)
                {
                    return ((XpoUnitOfWork)OuterUnitOfWork).Session;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
