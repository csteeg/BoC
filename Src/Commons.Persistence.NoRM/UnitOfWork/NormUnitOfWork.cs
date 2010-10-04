using BoC.UnitOfWork;
using Norm;

namespace BoC.Persistence.Norm.UnitOfWork
{
    public class NormUnitOfWork : BaseThreadSafeSingleUnitOfWork
    {
        private readonly ISessionFactory sessionFactory;
        private IMongo mongo;

        public NormUnitOfWork(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        override protected void CleanUp()
        {
            if (mongo == null) return;

            mongo.Dispose();
            mongo = null;
        }

        public IMongo Session
        {
            get
            {
                if (OuterUnitOfWork == this)
                {
                    return mongo ?? (mongo = sessionFactory.CreateSession());
                }
                return OuterUnitOfWork != null ? ((NormUnitOfWork)OuterUnitOfWork).Session : null;
            }
        }
    }
}
