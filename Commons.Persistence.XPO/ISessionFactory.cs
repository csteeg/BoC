using DevExpress.Xpo;

namespace BoC.Persistence.Xpo
{
    public interface ISessionFactory
    {
        Session OpenSession();
    }
}
