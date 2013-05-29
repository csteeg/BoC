using System;
using Sitecore.ContentSearch;
using Sitecore.Data.Items;
using Sitecore.Search;

namespace BoC.Persistence.SitecoreGlass
{
    public interface IProviderSearchContextProvider
    {
        IProviderSearchContext GetProviderSearchContext();
    }
}
