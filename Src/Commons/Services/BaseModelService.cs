using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.DomainServices;
using System.Web.Ria.Data;
using BoC.Persistence;
using BoC.Validation;

namespace BoC.Services
{
    public abstract class BaseModelService<TModel> : IModelService<TModel> where TModel : IBaseEntity
    {
        protected readonly IModelValidator validator;
        protected readonly IRepository<TModel> repository;
        // Methods
        public BaseModelService(IModelValidator validator, IRepository<TModel> repository)
        {
            this.validator = validator;
            this.repository = repository;
        }

        public virtual TModel Get(object id)
        {
            return repository.Get(id);
        }

        public virtual IQueryable<TModel> Query()
        {
            return repository;
        }

        public virtual TModel Insert(TModel entity)
        {
            TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required);
            using (transaction)
            {
                TModel retValue = repository.Save(entity);
                transaction.Complete();
                return retValue;
            }
        }

        public virtual void Delete(TModel entity)
        {
            TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required);
            using (transaction)
            {
                repository.Delete(entity);
                transaction.Complete();
            }
        }

        public virtual TModel Update(TModel entity)
        {
            TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required);
            using (transaction)
            {
                TModel retValue = repository.SaveOrUpdate(entity);
                transaction.Complete();
                return retValue;
            }
        }

        public virtual void ValidateEntity(TModel entity)
        {
            var errors = validator.Validate(entity);
            if (errors != null && errors.Count > 0)
                throw new RulesException(errors);
        }

        object IModelService.Get(object id)
        {
            return Get(id);
        }
    }
}