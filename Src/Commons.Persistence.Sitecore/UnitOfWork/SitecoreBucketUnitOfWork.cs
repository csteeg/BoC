using System;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Security;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace BoC.Persistence.SitecoreGlass.UnitOfWork
{
	public class SitecoreBucketUnitOfWork : SitecoreUnitOfWork
	{
		private Item _item;

		private SitecoreBucketUnitOfWork(Item item) : base(null)
		{
			_item = item;
		}

		public static SitecoreBucketUnitOfWork BeginUnitOfWork(Guid itemId, Database db)
		{
			return new SitecoreBucketUnitOfWork(db.GetItem(ID.Parse(itemId)));
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

		protected override void CleanUpOuterUnitOfWork()
		{
			_item = null;
			base.CleanUpOuterUnitOfWork();
		}
	}
}
