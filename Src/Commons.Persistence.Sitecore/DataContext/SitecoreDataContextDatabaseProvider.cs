using System.Globalization;
using BoC.DataContext;
using BoC.Persistence.SitecoreGlass.DataContext;
using Sitecore.Data;
using Sitecore;

namespace BoC.Persistence.SitecoreGlass
{
    public class SitecoreDataContextDatabaseProvider : IDatabaseProvider
    {
        public Database GetDatabase()
        {
            if (!(BaseThreadSafeSingleDataContext.CurrentDataContext is SitecoreDataContext))
            {
                throw new DataContextException("You are using SitecoreDataContextDatabaseProvider but are accessing a session outside a SitecoreDataContext");
            }
            return ((SitecoreDataContext)SitecoreDataContext.CurrentDataContext).GetDatabase();
        }

        public CultureInfo GetCulture()
        {
            if (!(BaseThreadSafeSingleDataContext.CurrentDataContext is SitecoreDataContext))
            {
                throw new DataContextException("You are using SitecoreDataContextDatabaseProvider but are accessing a session outside a SitecoreDataContext");
            }
            return ((SitecoreDataContext)SitecoreDataContext.CurrentDataContext).GetCultureInfo();
        }
    }
}
