using BoC.Persistence.SitecoreGlass;
using Glass.Mapper.Sc;


namespace BoC.Persistence.SitecoreGlass
{
    public class SitecoreServiceProvider : ISitecoreServiceProvider
    {
        private IDatabaseProvider databaseProvider;
        public SitecoreServiceProvider(IDatabaseProvider databaseProvider)
        {
            this.databaseProvider = databaseProvider;
        }

        public ISitecoreService GetSitecoreService()
        {
            return new SitecoreService(databaseProvider.GetDatabase());
        }
    }
}
