using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BoC.Persistence;

namespace MvcTodo.Models.Entity
{
    public class Catalog : BaseEntity<int>
    {
        public Catalog()
        {
            Todos = new List<Todo>();
        }

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public virtual IList<Todo> Todos { get; private set; }

    }
}
