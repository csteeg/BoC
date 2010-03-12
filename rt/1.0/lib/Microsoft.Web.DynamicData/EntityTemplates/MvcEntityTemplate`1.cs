namespace Microsoft.Web.DynamicData.Mvc {
    public class MvcEntityTemplate<TEntity> : MvcEntityTemplate where TEntity : class, new() {
        public new DynamicDataHelper<TEntity> DynamicData {
            get { return (DynamicDataHelper<TEntity>)base.DynamicData; }
        }

        public new TEntity Entity {
            get { return (TEntity)base.Entity; }
        }

        protected override DynamicDataHelper CreateDynamicData() {
            return new DynamicDataHelper<TEntity>(this);
        }
    }
}