using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BoC.Validation;
using NHibernate.Linq;

namespace BoC.Persistence.NHibernate
{
    public class NHRepository<T> : IQueryable<T>, IRepository<T> where T : class, IBaseEntity
    {
        private readonly ISessionManager sessionManager;

        public NHRepository(ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        #region IRepository Members

        virtual public T Get(object id)
        {
            return sessionManager.Session.Get<T>(id);
        }

        virtual public void Delete(T target)
        {
            sessionManager.Session.Delete(target);
        }

        virtual public void DeleteById(object id)
        {
            sessionManager.Session.CreateSQLQuery("delete from " + typeof(T).Name + " where id = ?").SetParameter(0, id).ExecuteUpdate();
        }

        virtual public T[] Query(Expression<System.Func<T, bool>> where)
        {
            return sessionManager.Session.Linq<T>().Where(where).ToArray();
        }

        #endregion

        virtual public T SaveOrUpdate(T target)
        {
            return sessionManager.Session.SaveOrUpdateCopy(target) as T;
        }

        virtual public T Save(T target)
        {
            sessionManager.Session.Save(target);
            return target;
        }

        virtual public T Update(T target)
        {
            sessionManager.Session.Update(target);
            return target;
        }

        virtual public T FindOne(Expression<System.Func<T, bool>> where)
        {
            return sessionManager.Session.Linq<T>().FirstOrDefault(where);
        }

        virtual public IQueryable<T> All()
        {
            return sessionManager.Session.Linq<T>();
        }

        virtual public void Evict(T target)
        {
            sessionManager.Session.Evict(target);
        }

         #region IEnumerable<T> Members

         IEnumerator<T> IEnumerable<T>.GetEnumerator()
         {
             return All().GetEnumerator();
         }

         #endregion

         #region IEnumerable Members

         System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
         {
             return All().GetEnumerator();
         }

         #endregion

         #region IQueryable Members

         Type IQueryable.ElementType
         {
             get { return All().ElementType; }
         }

         System.Linq.Expressions.Expression IQueryable.Expression
         {
             get { return All().Expression; }
         }

         IQueryProvider IQueryable.Provider
         {
             get { return All().Provider; }
         }

         #endregion

         #region IRepository Members

         object IRepository.Get(object id)
         {
             return this.Get(id);
         }

         void IRepository.Delete(object target)
         {
             this.Delete(target as T);
         }

         object IRepository.Save(object target)
         {
             return this.Save(target as T);
         }

         object IRepository.Update(object target)
         {
             return this.Update(target as T);
         }

         object IRepository.SaveOrUpdate(object target)
         {
             return SaveOrUpdate(target as T);
         }

         void IRepository.Evict(object target)
         {
             Evict(target as T);
         }

         #endregion
    }
}