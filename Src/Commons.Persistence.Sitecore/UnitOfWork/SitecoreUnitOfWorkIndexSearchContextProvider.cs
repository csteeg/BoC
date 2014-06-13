using Sitecore.ContentSearch;
using Sitecore.Search;

namespace BoC.Persistence.SitecoreGlass.UnitOfWork
{
    public class SitecoreUnitOfWorkIndexSearchContextProvider : IProviderSearchContextProvider
    {
        public IProviderSearchContext GetProviderSearchContext()
        {
            if (SitecoreUnitOfWork.OuterUnitOfWork == null)
            {
                throw new UnitOfWorkException("You are using SitecoreUnitOfWorkIndexProvider but are accessing a session outside a unit of work");
            }
            return ((SitecoreUnitOfWork)SitecoreUnitOfWork.CurrentUnitOfWork).IndexSearchContext;

        }
    }
}