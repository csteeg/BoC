using Glass.Mapper.Sc;

namespace BoC.Persistence.SitecoreGlass
{
    public interface ISitecoreServiceProvider
    {
        ISitecoreService GetSitecoreService();
    }

}
