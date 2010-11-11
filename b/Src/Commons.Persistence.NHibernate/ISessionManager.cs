using NHibernate;

namespace BoC.Persistence.NHibernate
{
    public interface ISessionManager
    {
        ISession Session { get; }
    }
}