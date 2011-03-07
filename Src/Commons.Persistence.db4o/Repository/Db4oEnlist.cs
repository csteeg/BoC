using System.Transactions;

using Db4objects.Db4o;

namespace BoC.Persistence.db4o.Repository
{
    public class Db4oEnlist : IEnlistmentNotification
    {
        private IObjectContainer container;
        private object oldItem;

        public Db4oEnlist(IObjectContainer container, object item)
        {
            this.container = container;
            oldItem = item;
        }

        #region IEnlistmentNotification

        public void Commit(Enlistment enlistment)
        {
            container.Commit();
            oldItem = null;
            container = null;
        }

        public void InDoubt(Enlistment enlistment)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        public void Rollback(Enlistment enlistment)
        {
            container.Rollback();
            container.Ext().Refresh(oldItem, int.MaxValue);
            oldItem = null;
            container = null;
        }

        #endregion
    }
}
