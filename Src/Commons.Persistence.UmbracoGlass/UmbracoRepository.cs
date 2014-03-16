using System;
using System.Linq;
using Glass.Mapper.Configuration;
using Umbraco.Core.Models;

namespace BoC.Persistence.UmbracoGlass
{
    public class UmbracoRepository<T> : IRepository<T> where T : class, IBaseEntity<int>, new()
    {
        private readonly IUmbracoServiceProvider _umbracoServiceProvider;
        private readonly ParentConfiguration _parentProperty;

        public UmbracoRepository(IUmbracoServiceProvider umbracoServiceProvider)
        {
            _umbracoServiceProvider = umbracoServiceProvider;

            var classInfo = _umbracoServiceProvider.GetUmbracoService().GlassContext[typeof(T)];
            _parentProperty = classInfo.Properties.OfType<ParentConfiguration>().FirstOrDefault();
        }

        public IQueryable<T> Query()
        {
            throw new NotImplementedException();
        }

        public T Save(T target)
        {
            target.Id = default(int);
            return SaveOrUpdate(target);
        }

        public T Update(T target)
        {
            return SaveOrUpdate(target);
        }

        public T SaveOrUpdate(T target)
        {
            var parent = GetParent(target);
            var umbracoService = _umbracoServiceProvider.GetUmbracoService();
            if (target.Id == 0)
            {
                target = umbracoService.Create(parent, target);
            }
            else
            {
                umbracoService.Save(target);
            }

            return target;
        }

        public virtual T Get(object id)
        {
            if (id is Guid) return _umbracoServiceProvider.GetUmbracoService().GetItem<T>((Guid) id, true, true);
            if (id is int) return _umbracoServiceProvider.GetUmbracoService().GetItem<T>((int) id, true, true);
            return null;
        }

        public void Delete(T target)
        {
            _umbracoServiceProvider.GetUmbracoService().Delete(target);
        }

        public void Evict(T target)
        {
            throw new NotImplementedException();
        }

        private IBaseEntity<int> GetParent(T model)
        {
            if (model == null) return null;

            if (_parentProperty != null)
            {
                return _parentProperty.PropertyInfo.GetValue(model, null) as IBaseEntity<int>;
            }

            if (model.Id != 0)
            {
                var item = _umbracoServiceProvider.GetUmbracoService().ContentService.GetById(model.Id);
                if (item != null && item.Parent() != null)
                {
                    var result = Get(item.Parent().Id);
                    if (result != null) return result;
                }
            }

            return null;
        }

        #region IRepository implementation

        object IRepository.Get(object id)
        {
            return Get(id);
        }

        object IRepository.Save(object target)
        {
            return SaveOrUpdate(target as T);
        }

        object IRepository.Update(object target)
        {
            return SaveOrUpdate(target as T);
        }

        object IRepository.SaveOrUpdate(object target)
        {
            return SaveOrUpdate(target as T);
        }

        void IRepository<T>.DeleteById(object id)
        {
            var item = ((IRepository<T>)this).Get(id);
            if (item != null) Delete(item);
        }

        void IRepository.Delete(object target)
        {
            Delete(target as T);
        }

        void IRepository.Evict(object target)
        {
            var t = target as T;
            if (t != null) Evict(t);
        }

        #endregion
    }
}
