using System;
using System.Linq;

namespace BoC.Persistence.UmbracoGlass
{
    public class UmbracoRepository<T> : IRepository<T> where T : class, IBaseEntity<Guid>, new()
    {
        private readonly IUmbracoServiceProvider _umbracoServiceProvider;

        public UmbracoRepository(IUmbracoServiceProvider umbracoServiceProvider)
        {
            _umbracoServiceProvider = umbracoServiceProvider;
        }

        public void Delete(T target)
        {
            throw new NotImplementedException();
        }

        public void DeleteById(object id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Query()
        {
            throw new NotImplementedException();
        }

        public T Save(T target)
        {
            throw new NotImplementedException();
        }

        public T Update(T target)
        {
            throw new NotImplementedException();
        }

        public T SaveOrUpdate(T target)
        {
            throw new NotImplementedException();
        }

        public void Evict(T target)
        {
            throw new NotImplementedException();
        }

        public virtual T Get(object id)
        {
            if (id is Guid) return _umbracoServiceProvider.GetUmbracoService().GetItem<T>((Guid) id, true, true);
            if (id is int) return _umbracoServiceProvider.GetUmbracoService().GetItem<T>((int) id, true, true);
            return null;
        }

        public void Delete(object target)
        {
            throw new NotImplementedException();
        }

        public object Save(object target)
        {
            throw new NotImplementedException();
        }

        public object Update(object target)
        {
            throw new NotImplementedException();
        }

        public object SaveOrUpdate(object target)
        {
            throw new NotImplementedException();
        }

        public void Evict(object target)
        {
            throw new NotImplementedException();
        }

        #region IRepository implementation

        T IRepository<T>.Get(object id)
        {
            return this.Get(id);
        }

        object IRepository.Get(object id)
        {
            return Get(id);
        }

        #endregion
    }
}
