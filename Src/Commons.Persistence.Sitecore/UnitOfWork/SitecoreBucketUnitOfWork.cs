using System;

namespace BoC.Persistence.SitecoreGlass.UnitOfWork
{
	public class SitecoreBucketUnitOfWork : SitecoreUnitOfWork
	{
		private readonly Item _item;

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
					this._index = ContentSearchManager.GetIndex(((SitecoreIndexableItem)_item).CreateSearchContext((SearchSecurityOptions) 2);
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
