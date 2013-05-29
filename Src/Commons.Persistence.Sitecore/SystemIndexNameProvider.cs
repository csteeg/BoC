using Sitecore.Data.Items;

namespace BoC.Persistence.SitecoreGlass
{
    public class SystemIndexNameProvider : IIndexNameProvider
    {
        public string GetIndexName()
        {
            return "system";
        }
    }
}
