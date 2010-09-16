using BoC.Persistence.Xpo.UnitOfWork;
using DevExpress.Xpo;

namespace BoC.Persistence.Xpo
{
    public class DefaultSessionFactory: ISessionFactory
    {
        public Session OpenSession()
        {
            return new Session();
        }
    }
}
