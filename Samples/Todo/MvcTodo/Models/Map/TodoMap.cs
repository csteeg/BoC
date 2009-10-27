using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Automapping.Alterations;
using MvcTodo.Models.Entity;

namespace MvcTodo.Models.Map
{
    public class WebsiteMap : IAutoMappingOverride<Todo>
    {
        #region IAutoMappingOverride<Menu> Members

        public void Override(FluentNHibernate.Automapping.AutoMapping<Todo> mapping)
        {
            mapping.References(m => m.Catalog).Cascade.All();
        }

        #endregion
    }
}
