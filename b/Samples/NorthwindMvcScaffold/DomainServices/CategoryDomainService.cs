/*using System;
using System.Linq;
using BoC.DomainServices;
using BoC.Persistence;
using BoC.Validation;
using NorthwindMvcScaffold.Models;

namespace NorthwindMvcScaffold.DomainServices
{
    public class CategoryDomainService: RepositoryDomainService<Category>
    {
        private readonly IRepository<Category> repository;

        public CategoryDomainService(IModelValidator validator, IRepository<Category> repository) : base(validator, repository)
        {
            this.repository = repository;
        }

        public override System.Linq.IQueryable<Category> Get()
        {
            return base.Get().Where(c => c.CategoryName != "test");
        }

        public override void ValidateEntity(object entity)
        {
            if (((Category)entity).CategoryName.Contains("test"))
                throw new Exception("We don't want any test categories any longer!");
            base.ValidateEntity(entity);
        }
    }
}*/