using System;
using System.Linq;
using System.Linq.Expressions;

namespace BoC.Persistence
{
    public interface IRepository
    {
        object Get(object id);
        void Delete(object target);
        object Save(object target);
        object Update(object target);
        object SaveOrUpdate(object target);
        void Evict(object target);
    }
    public interface IRepository<T>: IRepository, IQueryable<T> where T : IBaseEntity
    {
        T Get(object id);
        void Delete(T target);
        void DeleteById(object id);
        T[] Query(Expression<Func<T, bool>> where);
        T FindOne(Expression<Func<T, bool>> where);
        T Save(T target);
        T Update(T target);
        T SaveOrUpdate(T target);
        void Evict(T target);
    }
}