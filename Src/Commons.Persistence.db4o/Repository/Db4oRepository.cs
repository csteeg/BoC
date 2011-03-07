using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using BoC.Persistence;
using Db4objects.Db4o;
using Db4objects.Db4o.Linq;

namespace Commons.Persistence.db4o.Repository
{
    public class Db4oRepository<T> : IRepository<T> where T : class, IBaseEntity
    {
        private readonly ISessionManager _sessionManager;

        public Db4oRepository(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        object IRepository.Get(object id)
        {
            return Get(id);
        }

        void IRepository.Delete(object target)
        {
            Delete(target as T);
        }

        object IRepository.Save(object target)
        {
            return Save(target as T);
        }

        object IRepository.Update(object target)
        {
            return Update(target as T);
        }

        object IRepository.SaveOrUpdate(object target)
        {
            return SaveOrUpdate(target as T);
        }

        void IRepository.Evict(object target)
        {
            Evict(target as T);
        }

        public void Delete(T target)
        {
            var enlist = new Db4oEnlist(_sessionManager.Session, target);
            var inTransaction = Enlist(enlist);
            _sessionManager.Session.Delete(target);

            if (!inTransaction)
            {
                _sessionManager.Session.Commit();
            }
        }

        public void DeleteById(object id)
        {
            T obj = Get(id);
            if(obj != null)
            {
                Delete(obj);
            }
        }

        public IQueryable<T> Query()
        {
            return _sessionManager.Session.AsQueryable<T>();
        }

        public T Save(T target)
        {
            return SaveOrUpdate(target);
        }

        public T Update(T target)
        {
            _sessionManager.Session.Store(target);
            return target;
        }

        public T SaveOrUpdate(T target)
        {
            var enlist = new Db4oEnlist(_sessionManager.Session, target);
            bool inTransaction = Enlist(enlist);
            _sessionManager.Session.Store(target);
            if (!inTransaction)
            {
                _sessionManager.Session.Commit();
            }

            return target;
        }

        public void Evict(T target)
        {
            _sessionManager.Session.Ext().Purge(target);
        }

        public T Get(object id)
        {
            return (from T obj in _sessionManager.Session
                   where obj.Id == id
                   select obj).FirstOrDefault();
        }

        private static bool Enlist(IEnlistmentNotification enlist)
        {
            Transaction currentTransaction = Transaction.Current;
            if (currentTransaction != null)
            {
                currentTransaction.EnlistVolatile(enlist, EnlistmentOptions.None);
                return true;
            }
            return false;
        }
    }
}
