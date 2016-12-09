using BoC.DataContext;
using Sitecore.ContentSearch;
using Sitecore.Search;

namespace BoC.Persistence.SitecoreGlass.DataContext
{
    public class SitecoreDataContextIndexSearchContextProvider : IProviderSearchContextProvider
    {
        public IProviderSearchContext GetProviderSearchContext()
        {
            if (!(BaseThreadSafeSingleDataContext.CurrentDataContext is SitecoreDataContext))
            {
                throw new DataContextException("You are using SitecoreDataContextIndexProvider but are accessing a session outside a SitecoreDataContext");
            }
            return ((SitecoreDataContext)SitecoreDataContext.CurrentDataContext).IndexSearchContext;

        }
    }
}