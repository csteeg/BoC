using DevExpress.Xpo;

namespace BoC.Persistence.Xpo.UnitOfWork
{
    public class XpoUnitOfWorkSessionFactory: ISessionFactory
    {
        public Session OpenSession()
        {
            return new DevExpress.Xpo.UnitOfWork();
        }
    }
}
