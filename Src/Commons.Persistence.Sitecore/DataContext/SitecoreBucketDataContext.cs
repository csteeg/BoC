using System;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Security;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace BoC.Persistence.SitecoreGlass.DataContext
{
	public class SitecoreBucketDataContext : SitecoreDataContext
	{
		private Item _item;

		private SitecoreBucketDataContext(Item item) : base(null)
		{
			_item = item;
		}

		public static SitecoreBucketDataContext BeginDataContext(Guid itemId, Database db)
		{
			return new SitecoreBucketDataContext(db.GetItem(ID.Parse(itemId)));
		}

		public override IProviderSearchContext IndexSearchContext
		{
			get
			{
				if (this._index == null)
					this._index = ContentSearchManager.GetIndex((SitecoreIndexableItem)_item).CreateSearchContext();
				return this._index;
			}
		}

		protected override void CleanUpOuterDataContext()
		{
			_item = null;
			base.CleanUpOuterDataContext();
		}
	}
}
