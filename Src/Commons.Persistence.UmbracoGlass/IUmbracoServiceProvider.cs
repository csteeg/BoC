using Glass.Mapper.Umb;

namespace BoC.Persistence.UmbracoGlass
{
    public interface IUmbracoServiceProvider
    {
        IUmbracoService GetUmbracoService();
    }
}
