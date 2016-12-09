using System.Globalization;
using BoC.DataContext;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Search;

namespace BoC.Persistence.SitecoreGlass.DataContext
{
    public class SitecoreDataContext : BaseThreadSafeSingleDataContext
    {
        private readonly IIndexNameProvider _indexNameProvider;
        protected IProviderSearchContext _index;
        private string _database;
        private CultureInfo _cultureInfo;

        public SitecoreDataContext(IIndexNameProvider indexNameProvider): base()
        {
            _indexNameProvider = indexNameProvider;
        }

        public static SitecoreDataContext BeginDataContext(IIndexNameProvider indexNameProvider = null, string database = null, CultureInfo cultureInfo = null)
        {
            return new SitecoreDataContext(indexNameProvider)
            {
                _database = database,
                _cultureInfo = cultureInfo
            };
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
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

        protected override void CleanUpOuterDataContext()
        {
            
        }

        public virtual Database GetDatabase()
        {
            if (!string.IsNullOrEmpty(_database))
                return Database.GetDatabase(_database);
            if (OuterDataContext != this && OuterDataContext is SitecoreDataContext)
                return ((SitecoreDataContext) OuterDataContext).GetDatabase();
            return Context.Database ?? Context.ContentDatabase;
        }

        public CultureInfo GetCultureInfo()
        {
            if (_cultureInfo != null)
                return _cultureInfo;
            if (OuterDataContext != this && OuterDataContext is SitecoreDataContext)
            {
                return ((SitecoreDataContext) OuterDataContext).GetCultureInfo();
            }
            return Context.Language.CultureInfo;
        }


        public virtual IProviderSearchContext IndexSearchContext
        {
            get
            {
                if (OuterDataContext == this || !(OuterDataContext is SitecoreDataContext) || ((SitecoreDataContext)OuterDataContext)._indexNameProvider.GetIndexName() != (_indexNameProvider == null ? null : _indexNameProvider.GetIndexName()))
                {
                    if (_indexNameProvider != null)
                        return _index ??
                               (_index = ContentSearchManager.GetIndex(_indexNameProvider.GetIndexName()).CreateSearchContext());
                }
                else if (OuterDataContext is SitecoreDataContext)
                {
                    return ((SitecoreDataContext)OuterDataContext).IndexSearchContext;
                }
                return null;
            }
        }

    }
}
