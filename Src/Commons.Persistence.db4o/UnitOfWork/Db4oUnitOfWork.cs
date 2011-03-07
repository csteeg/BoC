using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using BoC.UnitOfWork;
using Db4objects.Db4o;

namespace BoC.Persistence.db4o.UnitOfWork
{
    public class Db4oUnitOfWork : BaseThreadSafeSingleUnitOfWork
    {
        private readonly ISessionFactory _sessionFactory;
        private IObjectContainer _sessionObjectContainer;

        public Db4oUnitOfWork(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        protected override void CleanUpOuterUnitOfWork()
        {
            if (_sessionObjectContainer == null)
                return;

            try
            {
                if (Transaction.Current != null &&
                    Transaction.Current.TransactionInformation.Status == TransactionStatus.Active)
                {
                    //TODO: find out if whats better, the Enlisted object will prolly need a refresh...
                    //_objectContainer.Rollback();
                    Transaction.Current.Rollback();
                }
            }
            finally
            {
                _sessionObjectContainer.Close();
                _sessionObjectContainer.Dispose();
                _sessionObjectContainer = null;
            }
        }

        public IObjectContainer Session
        {
            get
            {
                if (OuterUnitOfWork == this)
                {
                    return _sessionObjectContainer ?? (_sessionObjectContainer = _sessionFactory.CreateSession());
                }
                else if (OuterUnitOfWork != null)
                {
                    return ((Db4oUnitOfWork)OuterUnitOfWork).Session;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
