using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using BoC.EventAggregator;
using BoC.Events;
using BoC.Persistence;
using BoC.Validation;

namespace BoC.Services
{
    public abstract class BaseModelService<TModel> : IModelService<TModel> where TModel : IBaseEntity
    {
        protected readonly IModelValidator validator;
        private readonly IEventAggregator eventAggregator;
        protected readonly IRepository<TModel> repository;
        // Methods
        public BaseModelService(
            IModelValidator validator, 
            IEventAggregator eventAggregator, 
            IRepository<TModel> repository)
        {
            this.validator = validator;
            this.eventAggregator = eventAggregator;
            this.repository = repository;
        }

        public virtual TModel Get(object id)
        {
            return repository.Get(id);
        }

        public virtual IEnumerable<TModel> ListAll()
        {
            foreach (var entity in repository)
            {
                yield return entity;
            }
        }

        public virtual IQueryable<TModel> Query()
        {
            return repository;
        }

        public virtual TModel Insert(TModel entity)
        {
            OnInserting(entity);

            TModel retValue = default(TModel);
            ExecuteInTransaction(() => { retValue = repository.Save(entity); });

            OnInserted(retValue);

            return retValue;
        }

        public virtual void Delete(TModel entity)
        {
            OnDeleting(entity);

            ExecuteInTransaction(() => repository.Delete(entity));

            OnDeleted(entity);
        }

        public virtual TModel Update(TModel entity)
        {
            OnUpdating(entity);

            TModel retValue = default(TModel);
            ExecuteInTransaction(() => { retValue = repository.SaveOrUpdate(entity); });

            OnUpdated(retValue);

            return retValue;
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

        private void ExecuteInTransaction(Action action)
        {
            using(var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                action();
                transaction.Complete();
            }
        }

        #region Events

        protected void OnInserting(TModel entity)
        {
            PublishEvent<InsertingEvent<TModel>>(entity);
        }

        protected void OnInserted(TModel entity)
        {
            PublishEvent<InsertedEvent<TModel>>(entity);
        }

        protected void OnUpdating(TModel entity)
        {
            PublishEvent<UpdatingEvent<TModel>>(entity);
        }

        protected void OnUpdated(TModel entity)
        {
            PublishEvent<UpdatedEvent<TModel>>(entity);
        }

        protected void OnDeleting(TModel entity)
        {
            PublishEvent<DeletingEvent<TModel>>(entity);
        }

        protected void OnDeleted(TModel entity)
        {
            PublishEvent<DeletedEvent<TModel>>(entity);
        }

        protected void PublishEvent<TEvent>(TModel entity) where TEvent : BaseEvent<EventArgs<TModel>>, new()
        {
            EventArgs<TModel> args = new EventArgs<TModel>(entity);
            eventAggregator.GetEvent<TEvent>().Publish(args);
        }

        #endregion
    }
}