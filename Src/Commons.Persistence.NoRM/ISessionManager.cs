using Norm;

namespace BoC.Persistence.Norm
{
    public interface ISessionManager
    {
        IMongo Session { get; }
    }
}