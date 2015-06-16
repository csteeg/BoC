using BoC.InversionOfControl;

namespace BoC.DataContext
{
    public static class DataContext
    {
        public static IDataContext BeginDataContext()
        {
            if (IoC.IsInitialized())
                return IoC.Resolver.Resolve<IDataContext>() ?? new DummyDataContext();
            return new DummyDataContext();
        }
    }

    class DummyDataContext : IDataContext
    {
        public void Dispose()
        {
            
        }
   }
}