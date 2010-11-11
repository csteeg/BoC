using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using BoC.Persistence;

namespace BoC.Services
{
    public static class ModelQuery
    {
        public static ModelQuery<TModel> Where<TModel>(Expression<Func<TModel, bool>> where) where TModel : IBaseEntity
        {
            return new ModelQuery<TModel>() {Expression = where };
        }
    }
    public class ModelQuery<TModel> where TModel : IBaseEntity
    {
        public virtual Expression<Func<TModel, bool>> Expression { get; set; }
        public virtual int ItemsToSkip { get; set; }
        public virtual int ItemsToTake { get; set; }
        public virtual string OrderByExpression { get; set; }

        public ModelQuery<TModel> Take(int take)
        {
            this.ItemsToTake = take;
            return this;
        }

        public ModelQuery<TModel> Skip(int skip)
        {
            this.ItemsToSkip = skip;
            return this;
        }

        public ModelQuery<TModel> OrderBy(string orderBy)
        {
            OrderByExpression = orderBy;
            return this;
        }
    }
}
