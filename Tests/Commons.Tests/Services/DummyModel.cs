using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BoC.Persistence;
using BoC.Services;

namespace BoC.Tests.Services
{
    public class DummyModel: IBaseEntity
    {
        public bool Equals(IBaseEntity other) { return Id == other.Id; }

        public object Id { get; set; }
    }

    public class DummyModel2 : IBaseEntity
    {
        public bool Equals(IBaseEntity other) { return Id == other.Id; }

        public object Id { get; set; }
    }

    public class DummyModelService : IModelService<DummyModel>
    {
        object IModelService.Get(object id)
        {
            return Get(id);
        }

        public DummyModel Insert(DummyModel entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(DummyModel entity)
        {
            throw new NotImplementedException();
        }

        public DummyModel Update(DummyModel entity)
        {
            throw new NotImplementedException();
        }

        public void ValidateEntity(DummyModel entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DummyModel> ListAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DummyModel> Find(Expression<Func<DummyModel, bool>> where)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DummyModel> Find(ModelQuery<DummyModel> query)
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<DummyModel, bool>> where)
        {
            throw new NotImplementedException();
        }

        public DummyModel Get(object id)
        {
            throw new NotImplementedException();
        }
    }

    public class DummyModelService2 : IDummyModelService2{
        object IModelService.Get(object id)
        {
            return Get(id);
        }

        public DummyModel2 Insert(DummyModel2 entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(DummyModel2 entity)
        {
            throw new NotImplementedException();
        }

        public DummyModel2 Update(DummyModel2 entity)
        {
            throw new NotImplementedException();
        }

        public void ValidateEntity(DummyModel2 entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DummyModel2> ListAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DummyModel2> Find(Expression<Func<DummyModel2, bool>> where)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DummyModel2> Find(ModelQuery<DummyModel2> query)
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<DummyModel2, bool>> where)
        {
            throw new NotImplementedException();
        }

        public DummyModel2 Get(object id)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class BaseDummyModelService<T> : IModelService<T> where T : IBaseEntity
    {
        object IModelService.Get(object id)
        {
            return Get(id);
        }

        public T Insert(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public T Update(T entity)
        {
            throw new NotImplementedException();
        }

        public void ValidateEntity(T entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> ListAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> where)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Find(ModelQuery<T> query)
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<T, bool>> where)
        {
            throw new NotImplementedException();
        }

        public T Get(object id)
        {
            throw new NotImplementedException();
        }
    }
    public class InheritedDummyModelService: BaseDummyModelService<DummyModel>
    {}

    public interface IDummyModelService2 : IModelService<DummyModel2>
    {}
}