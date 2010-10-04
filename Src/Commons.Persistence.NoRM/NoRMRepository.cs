using System;
using System.Linq;
using Norm;

namespace BoC.Persistence.Norm
{
    public class NormRepository<T> : IRepository<T> where T : class, IBaseEntity
    {
        private readonly ISessionManager sessionManager;

        public NormRepository(ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        #region IRepository Members

        virtual public T Get(object id)
        {
            return TryCatch(() => Query().Where(o => o.Id == id).FirstOrDefault());
        }

        virtual public void Delete(T target)
        {
            TryCatch(() =>
                         {
                             sessionManager.Session.Database.GetCollection<T>().Delete(target);
                             return null;
                         });
        }

        virtual public void DeleteById(object id)
        {
            TryCatch(() =>
                         {
                             Delete(Get(id));
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
            return Save(target);
        }

        virtual public T Save(T target)
        {
            return TryCatch(() =>
            {
                sessionManager.Session.Database.GetCollection<T>().Save(target);
                return target;
            });
        }

        virtual public T Update(T target)
        {
            return TryCatch(() =>
            {
                sessionManager.Session.Database.GetCollection<T>().UpdateOne(target, target);
                return target;
            });
        }

        virtual public IQueryable<T> Query()
        {
            return sessionManager.Session.Database.GetCollection<T>().AsQueryable();
        }

        virtual public void Evict(T target)
        {
            //sessionManager.Session..Evict(target);
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