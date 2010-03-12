using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BoC.Persistence;

namespace MvcTodo.Models.Entity
{
    public class Todo : BaseEntity<int>
    {
        public virtual Catalog Catalog { get; set; }
        public virtual string Action { get; set; }
    }
}
