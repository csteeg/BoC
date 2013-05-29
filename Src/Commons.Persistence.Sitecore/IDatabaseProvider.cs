using System.Globalization;
using Sitecore.Data;

namespace BoC.Persistence.SitecoreGlass
{
    public interface IDatabaseProvider
    {
        Database GetDatabase();
        CultureInfo GetCulture();
    }
}
