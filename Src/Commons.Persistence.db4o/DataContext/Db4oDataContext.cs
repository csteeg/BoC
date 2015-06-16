using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using BoC.DataContext;
using Db4objects.Db4o;

namespace BoC.Persistence.db4o.DataContext
{
    public class Db4oDataContext : BaseThreadSafeSingleDataContext
    {
        private readonly ISessionFactory _sessionFactory;
        private IObjectContainer _sessionObjectContainer;

        public Db4oDataContext(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        protected override void CleanUpOuterDataContext()
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
                if (OuterDataContext == this)
                {
                    return _sessionObjectContainer ?? (_sessionObjectContainer = _sessionFactory.CreateSession());
                }
                else if (OuterDataContext != null)
                {
                    return ((Db4oDataContext)OuterDataContext).Session;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
