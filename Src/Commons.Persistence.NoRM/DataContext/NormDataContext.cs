using BoC.DataContext;
using Norm;

namespace BoC.Persistence.Norm.DataContext
{
    public class NormDataContext : BaseThreadSafeSingleDataContext
    {
        private readonly ISessionFactory sessionFactory;
        private IMongo mongo;

        public NormDataContext(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        override protected void CleanUpOuterDataContext()
        {
            if (mongo == null) return;

            mongo.Dispose();
            mongo = null;
        }

        public IMongo Session
        {
            get
            {
                if (OuterDataContext == this)
                {
                    return mongo ?? (mongo = sessionFactory.CreateSession());
                }
                return OuterDataContext != null ? ((NormDataContext)OuterDataContext).Session : null;
            }
        }
    }
}
