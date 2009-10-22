using System;
using BoC.Persistence.NHibernate;
using NorthwindMvcScaffold.Models;

namespace NorthwindMvcScaffold.Repositories
{
    public class ProductRepository: NHRepository<Product>
    {
        public ProductRepository(ISessionManager sessionManager) : base(sessionManager)
        {
        }

        public override Product Save(Product target)
        {
            if (target.Discontinued)
                throw new Exception("You cannot insert discontinued products");
            return base.Save(target);
        }
    }
}