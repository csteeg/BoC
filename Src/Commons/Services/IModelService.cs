using System.Collections.Generic;
using System.Linq;
using BoC.Persistence;

namespace BoC.Services
{
    public interface IModelService<TModel> : IModelService where TModel : IBaseEntity
    {
        new TModel Get(object id);
        IQueryable<TModel> Query();
        
        TModel Insert(TModel entity);
        void Delete(TModel entity);
        TModel Update(TModel entity);
        void ValidateEntity(TModel entity);
        IEnumerable<TModel> ListAll();
    }

    public interface IModelService
    {
        object Get(object id);
    }
 }
