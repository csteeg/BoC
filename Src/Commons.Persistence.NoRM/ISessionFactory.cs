using Norm;

namespace BoC.Persistence.Norm
{
    public interface ISessionFactory
    {
        IMongo CreateSession();
    }
}
