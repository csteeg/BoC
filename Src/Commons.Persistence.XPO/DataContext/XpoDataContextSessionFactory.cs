using DevExpress.Xpo;

namespace BoC.Persistence.Xpo.DataContext
{
    public class XpoDataContextSessionFactory: ISessionFactory
    {
        public Session OpenSession()
        {
            return new DevExpress.Xpo.UnitOfWork();
        }
    }
}
