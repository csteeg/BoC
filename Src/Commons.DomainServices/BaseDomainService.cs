using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.DomainServices;
using System.Web.Ria.Data;
using BoC.Validation;
using BoC.DomainServices.MetaData;

namespace BoC.DomainServices
{
    [IoCMetaDataProvider]
    public abstract class BaseDomainService : DomainService
    {
        protected readonly IModelValidator validator;
        // Methods
        protected BaseDomainService(IModelValidator validator)
        {
            this.validator = validator;
        }

        protected override int Count<T>(IQueryable<T> query)
        {
            return query.Count<T>();
        }

        public virtual void ValidateEntity(object entity)
        {
            var errors = validator.Validate(entity);
            if (errors != null && errors.Count > 0)
                throw new RulesException(errors);
        }

        protected override bool ValidateChangeSet(ChangeSet changeSet)
        {
            var isValid = base.ValidateChangeSet(changeSet);

            if (!(validator is DataAnnotationsModelValidator))//the default validator of domainservice does the data-annotations validation already
            {
                foreach (var operation in changeSet.EntityOperations)
                {
                    var errors = validator.Validate(operation.Entity);
                    if (errors != null && errors.Count > 0)
                    {
                        isValid = false;
                        List<OperationError> validationErrors;
                        if (operation.ValidationErrors is List<OperationError>)
                            validationErrors = (List<OperationError>) operation.ValidationErrors;
                        else if (operation.ValidationErrors != null)
                            validationErrors = new List<OperationError>(operation.ValidationErrors);
                        else
                            validationErrors = new List<OperationError>(errors.Count);

                        validationErrors.AddRange(
                            from error in errors
                            select new OperationError(error.ErrorMessage, error.PropertyName)
                            );
                        operation.ValidationErrors = validationErrors;
                    }
                }
            }

            return isValid;
        }

    }

 
}
