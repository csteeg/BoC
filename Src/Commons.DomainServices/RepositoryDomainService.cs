using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.DomainServices;
using System.Web.Ria.Data;
using BoC.Persistence;
using BoC.Validation;

namespace BoC.DomainServices
{
    public class RepositoryDomainService<TEntity> : BaseDomainService, IRepositoryDomainService<TEntity> where TEntity: class, IBaseEntity
    {
        private readonly IRepository<TEntity> repository;
        // Methods
        public RepositoryDomainService(IModelValidator validator, IRepository<TEntity> repository): base(validator)
        {
            this.repository = repository;
        }

        [Query(IsComposable = true)]
        virtual public IQueryable<TEntity> Get()
        {
            return repository;
        }

        virtual public void Insert(TEntity entity)
        {
            try
            {
                ValidateEntity(entity);
            }
            catch
            {
                repository.Evict(entity);
                throw;
            }
            this.repository.Save(entity);
        }

        virtual public void Update(TEntity entity)
        {
            try
            {
                ValidateEntity(entity);
            }
            catch
            {
                repository.Evict(entity);
                throw;
            }
            this.repository.Update(entity);
        }

        virtual public void Delete(TEntity entity)
        {
            this.repository.Delete(entity);
        }

    }

 
}
