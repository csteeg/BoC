using System.Transactions;
using BoC.UnitOfWork;
using DevExpress.Xpo;

namespace BoC.Persistence.Xpo.UnitOfWork
{
    public class XpoUnitOfWork : BaseThreadSafeSingleUnitOfWork
    {
        private readonly ISessionFactory sessionFactory;
        private Session session;
        private NestedUnitOfWork nestedUnitOfWOrk;

        public XpoUnitOfWork(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
            if (OuterUnitOfWork != this)
            {
                nestedUnitOfWOrk = Session.BeginNestedUnitOfWork();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (nestedUnitOfWOrk != null)
            {
                nestedUnitOfWOrk.CommitChanges();
                nestedUnitOfWOrk = null;
            }
            base.Dispose(disposing);
        }

        override protected void CleanUpOuterUnitOfWork()
        {
            if (session != null)
            {
                try
                {
                    if (session is DevExpress.Xpo.UnitOfWork)
                    {
                        ((DevExpress.Xpo.UnitOfWork)session).CommitChanges();
                    } 
                    else if (session.InTransaction) //
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
                    return session ?? (session = sessionFactory.OpenSession());
                }
                if (OuterUnitOfWork != null)
                {
                    return ((XpoUnitOfWork)OuterUnitOfWork).Session;
                }
                return null;
            }
        }
    }
}
