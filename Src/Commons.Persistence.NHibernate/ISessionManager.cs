using NHibernate;

namespace BoC.Persistence.NHibernate
{
    public interface ISessionManager
    {
        ISessionFactory SessionFactory { get; }
        ISession Session { get; }
        void CleanUp();
    }
}