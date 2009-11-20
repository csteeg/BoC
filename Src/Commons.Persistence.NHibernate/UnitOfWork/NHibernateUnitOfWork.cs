using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using BoC.UnitOfWork;
using NHibernate;

namespace BoC.Persistence.NHibernate.UnitOfWork
{
    public class NHibernateUnitOfWork: IUnitOfWork
    {
        private readonly ISessionManager _sessionManager;
        private readonly TransactionScope _transactionScope = new TransactionScope();
        
        [ThreadStatic]
        private static IUnitOfWork outerUnitOfWork = null;

        public NHibernateUnitOfWork(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            if (outerUnitOfWork == null)
            {
                outerUnitOfWork = this;
            }
        }

        public void Complete()
        {
            _transactionScope.Complete();
        }

        public void Dispose()
        {
            if (_transactionScope != null)
            {
                _transactionScope.Dispose();
            }
            if (outerUnitOfWork == this)
            {
                _sessionManager.CleanUp();
                outerUnitOfWork = null;
            }
        }
    }
}
