using BoC.UnitOfWork;
using Sitecore.ContentSearch;
using Sitecore.Search;

namespace BoC.Persistence.SitecoreGlass.UnitOfWork
{
    public class SitecoreUnitOfWork : BaseThreadSafeSingleUnitOfWork
    {
        private readonly IIndexNameProvider _indexNameProvider;
        private IProviderSearchContext _index;
        public SitecoreUnitOfWork(IIndexNameProvider indexNameProvider)
        {
            _indexNameProvider = indexNameProvider;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        override protected void CleanUpOuterUnitOfWork()
        {
            if (_index != null)
            {
                try
                {
                    _index.Dispose();
                }
                finally
                {
                    _index = null;
                }
            }
        }

        public IProviderSearchContext IndexSearchContext
        {
            get
            {
                if (OuterUnitOfWork == this)
                {
                    if (_index == null)
                    {
                        _index = ContentSearchManager.GetIndex(_indexNameProvider.GetIndexName()).CreateSearchContext();
                    }
                    return _index;
                }
                else if (OuterUnitOfWork != null)
                {
                    return ((SitecoreUnitOfWork)OuterUnitOfWork).IndexSearchContext;
                }
                else
                {
                    return null;
                }
            }
        }


    }
}
