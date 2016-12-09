using Sitecore;

namespace BoC.Persistence.SitecoreGlass
{
    public class ContentSearchContextIndexNameProvider : IIndexNameProvider
    {
        public string GetIndexName()
        {
            var currentDatabase = Context.Database ?? Context.ContentDatabase;;

            return currentDatabase == null
                        ? string.Empty
                        : string.Format("sitecore_{0}_index", currentDatabase.Name);
        }
    }
}
