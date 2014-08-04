using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using BoC.Validation;
using NHibernate.Linq;
using NHibernate.Transform;

namespace BoC.Persistence.NHibernate
{
    public class NHRepository<T> : IRepository<T> where T : class, IBaseEntity
    {
        private readonly ISessionManager sessionManager;

        public NHRepository(ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        #region IRepository Members

        virtual public T Get(object id)
        {
            return TryCatch(() => sessionManager.Session.Get<T>(id));
        }

        virtual public void Delete(T target)
        {
            TryCatch(() =>
                         {
                             sessionManager.Session.Delete(target);
                             return null;
                         });
        }

        virtual public void DeleteById(object id)
        {
            TryCatch(() =>
                         {
                             sessionManager.Session.CreateSQLQuery("delete from " + typeof (T).Name + " where id = ?").
                                 SetParameter(0, id).ExecuteUpdate();
                             return null;
                         });
        }
        
        #endregion

        protected virtual T TryCatch(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (UnresolvableObjectException exception)
            {
                throw new BoC.Persistence.Exceptions.ObjectNotFoundException(exception.Message, exception);
            }
            catch (NonUniqueObjectException exception)
            {
                throw new BoC.Persistence.Exceptions.ObjectNotUniqueException(exception.Message, exception);
            }
            catch (Exception exception)
            {
                if (!String.IsNullOrEmpty(exception.Message) && exception.Message.Contains("Cannot insert duplicate key in object"))
                {
                    throw new BoC.Persistence.Exceptions.ObjectNotUniqueException(exception.Message, exception);
                }
                if (exception.InnerException != null && !String.IsNullOrEmpty(exception.InnerException.Message) 
                    && exception.InnerException.Message.Contains("Cannot insert duplicate key in object"))
                {
                    throw new BoC.Persistence.Exceptions.ObjectNotUniqueException(exception.Message, exception);
                }
                throw;
            }
        }

        virtual public T SaveOrUpdate(T target)
        {
            return TryCatch(() => sessionManager.Session.SaveOrUpdateCopy(target) as T);
        }

        virtual public T Save(T target)
        {
            return TryCatch(() =>
                                {
                                    sessionManager.Session.Save(target);
                                    return target;
                                });
        }

        virtual public T Update(T target)
        {
            return TryCatch(() =>
                                {
                                    sessionManager.Session.Update(target);
                                    return target;
                                });
        }

        virtual public IQueryable<T> Query()
        {
            return sessionManager.Session.Query<T>();
        }

        virtual public void Evict(T target)
        {
            sessionManager.Session.Evict(target);
        }

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