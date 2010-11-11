using DevExpress.Xpo;

namespace BoC.Persistence.Xpo
{
    public interface ISessionManager
    {
        Session Session { get; }
    }
}