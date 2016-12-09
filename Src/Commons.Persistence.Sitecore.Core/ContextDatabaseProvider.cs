using System.Globalization;
using Sitecore.Data;
using Sitecore;

namespace BoC.Persistence.SitecoreGlass
{
    public class ContextDatabaseProvider : IDatabaseProvider
    {
        public Database GetDatabase()
        {
            return Context.Database ?? Context.ContentDatabase;
        }

        public CultureInfo GetCulture()
        {
            return Context.Language.CultureInfo;
        }
    }
}
