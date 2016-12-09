using System.Globalization;
using Sitecore.Data;
using Sitecore;

namespace BoC.Persistence.SitecoreGlass
{
    public class CustomDatabaseProvider : IDatabaseProvider
    {
        private readonly string _dbName;

        public CustomDatabaseProvider(string dbName, string cultureName = null)
        {
            _dbName = dbName;
            if (cultureName != null)
            {
                Culture = CultureInfo.GetCultureInfo(cultureName);
            }
        }

        public Database GetDatabase()
        {
            return Database.GetDatabase(_dbName);
        }

        public CultureInfo Culture { get; set; }

        public CultureInfo GetCulture()
        {
            return Culture ?? Context.Language.CultureInfo;
        }
    }
}
