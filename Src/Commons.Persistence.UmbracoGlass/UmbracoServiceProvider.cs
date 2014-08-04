using Glass.Mapper.Umb;
using Umbraco.Core.Services;

namespace BoC.Persistence.UmbracoGlass
{
    public class UmbracoServiceProvider : IUmbracoServiceProvider
    {
        public IUmbracoService GetUmbracoService()
        {
            return new UmbracoService(new ContentService());
        }
    }
}
