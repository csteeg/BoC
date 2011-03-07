using System;
using System.Linq;
using System.Transactions;
using Db4objects.Db4o.Linq;

namespace BoC.Persistence.db4o.Repository
{
    public class Db4oRepository<T> : IRepository<T> where T : class, IBaseEntity
    {
        private readonly ISessionManager sessionManager;

        public Db4oRepository(ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
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
            if (target == null)
                return; // Silently fail.

            TryCatch(() =>
                         {
                             if (!sessionManager.Session.Ext().IsActive(target))
                             {
                                 target = Get(target.Id);
                             }

                             var enlist = new Db4oEnlist(sessionManager.Session, target);
                             var inTransaction = Enlist(enlist);
                             sessionManager.Session.Delete(target);

                             if (!inTransaction)
                             {
                                 sessionManager.Session.Commit();
                             }
                             return null;
                         });
        }

        public void DeleteById(object id)
        {
            TryCatch(() =>
                         {
                             T obj = Get(id);
                             if (obj != null)
                             {
                                 Delete(obj);
                             }
                             return null;
                         });
        }

        public IQueryable<T> Query()
        {
            return sessionManager.Session.AsQueryable<T>();
        }

        public T Save(T target)
        {
            return TryCatch(() => SaveOrUpdate(target));
        }

        public T Update(T target)
        {
            return TryCatch(() =>
                                {
                                    if (!sessionManager.Session.Ext().IsStored(target))
                                    {
                                        //TODO: enforce updates / saves?
                                        return SaveOrUpdate(target);
                                    }
                                    else
                                    {
                                        return SaveOrUpdate(target);
                                    }
                                });
        }

        public T SaveOrUpdate(T target)
        {
            return TryCatch(() =>
                                {
                                    var enlist = new Db4oEnlist(sessionManager.Session, target);
                                    bool inTransaction = Enlist(enlist);
                                    sessionManager.Session.Store(target);
                                    if (!inTransaction)
                                    {
                                        sessionManager.Session.Commit();
                                    }

                                    return target;
                                });
        }

        public void Evict(T target)
        {
            sessionManager.Session.Ext().Purge(target);
        }

        public T Get(object id)
        {
            return TryCatch(() => (from obj in Query()
                                   where obj.Id.Equals(id)
                                   select obj).FirstOrDefault());
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

        protected virtual T TryCatch(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                //TODO: convert to BoC exceptions
                
                throw;
            }
        }
    }
}
