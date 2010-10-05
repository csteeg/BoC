using System;
using System.Linq;
using BoC.Persistence.Xpo;
using DevExpress.Xpo;

namespace BoC.Persistence.NHibernate
{
    public class XpoRepository<T> : IRepository<T> where T : class, IBaseEntity
    {
        private readonly ISessionManager sessionManager;

        public XpoRepository(ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        #region IRepository Members

        virtual public T Get(object id)
        {
            return TryCatch(() => sessionManager.Session.GetObjectByKey<T>(id));
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
                             sessionManager.Session.Delete(Get(id));
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
            catch (DevExpress.Xpo.Exceptions.TypeNotFoundException exception)
            {
                throw new BoC.Persistence.Exceptions.ObjectNotFoundException(exception.Message, exception);
            }
            catch (DevExpress.Xpo.Exceptions.DuplicateKeyPropertyException exception)
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
            return Save(target);
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
            return Save(target);
        }

        virtual public IQueryable<T> Query()
        {
            return new XPQuery<T>(sessionManager.Session);
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
