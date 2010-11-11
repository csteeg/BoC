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

        protected override void Dispose(bool disposing)
        {
            if (OuterUnitOfWork != this && Session != null && SafeIsDirty(Session))
            {
                Session.Flush();
            }
            base.Dispose(disposing);
        }

        private bool SafeIsDirty(ISession session1)
        {
            var isDirty = false;
            try
            {
                isDirty = Session.IsDirty();
            }
            catch (AssertionFailure)
            {
                isDirty = false; //cannot flush, since something is wrong with the objects (an exception occured already during this unitofwork)
            }
            return isDirty;
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
                    else if (SafeIsDirty(session))
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
                    if (session == null)
                    {
                        session = sessionFactory.OpenSession();
                    }
                    return session;
                }
                else if (OuterUnitOfWork != null)
                {
                    return ((NHibernateUnitOfWork)OuterUnitOfWork).Session;
                }
                else
                {
                    return null;
                }
            }
        }


    }
}
