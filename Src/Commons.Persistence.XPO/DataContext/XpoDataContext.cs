using System.Transactions;
using BoC.DataContext;
using DevExpress.Xpo;

namespace BoC.Persistence.Xpo.DataContext
{
    public class XpoDataContext : BaseThreadSafeSingleDataContext
    {
        private readonly ISessionFactory sessionFactory;
        private Session session;
        private NestedUnitOfWork nestedDataContext;

        public XpoDataContext(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        protected override void Dispose(bool disposing)
        {
            if (nestedDataContext != null)
            {
                nestedDataContext.CommitChanges();
                nestedDataContext = null;
            }
            base.Dispose(disposing);
        }

        override protected void CleanUpOuterDataContext()
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
                if (OuterDataContext == this)
                {
                    return session ?? (session = sessionFactory.OpenSession());
                }
                if (OuterDataContext != null)
                {
                    return nestedDataContext ?? (nestedDataContext = Session.BeginNestedUnitOfWork());
                }
                return null;
            }
        }
    }
}
