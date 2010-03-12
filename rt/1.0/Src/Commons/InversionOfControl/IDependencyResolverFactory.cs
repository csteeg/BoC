namespace BoC.InversionOfControl
{
    public interface IDependencyResolverFactory
    {
        IDependencyResolver CreateInstance();
    }
}