using BoC.DataContext;
using NHibernate;

namespace BoC.Persistence.NHibernate.DataContext
{
    public class NHibernateDataContext : BaseThreadSafeSingleDataContext
    {
        private readonly ISessionFactory sessionFactory;
        private ISession session;

        public NHibernateDataContext(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        protected override void Dispose(bool disposing)
        {
            if (OuterDataContext != this && Session != null && SafeIsDirty(Session))
            {
                Session.Flush();
            }
            base.Dispose(disposing);
        }

        private static bool SafeIsDirty(ISession sess)
        {
            var isDirty = false;
            try
            {
                isDirty = sess.IsDirty();
            }
            catch (AssertionFailure)
            {
                isDirty = false; //cannot flush, since something is wrong with the objects (an exception occured already during this DataContext)
            }
            return isDirty;
        }


        override protected void CleanUpOuterDataContext()
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
                if (OuterDataContext == this)
                {
                    if (session == null)
                    {
                        session = sessionFactory.OpenSession();
                    }
                    return session;
                }
                else if (OuterDataContext != null)
                {
                    return ((NHibernateDataContext)OuterDataContext).Session;
                }
                else
                {
                    return null;
                }
            }
        }


    }
}
