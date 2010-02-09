using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BoC.Persistence;

namespace BoC.Services
{
    public interface IModelService<TModel> : IModelService where TModel : IBaseEntity
    {
        new TModel Get(object id);
        TModel Insert(TModel entity);
        void Delete(TModel entity);
        TModel Update(TModel entity);
        void ValidateEntity(TModel entity);
        IEnumerable<TModel> ListAll();
        IEnumerable<TModel> Find(Expression<Func<TModel, bool>> where);
        IEnumerable<TModel> Find(ModelQuery<TModel> query);
        int Count(Expression<Func<TModel, bool>> where);
    }

    public interface IModelService
    {
        object Get(object id);
    }
 }
