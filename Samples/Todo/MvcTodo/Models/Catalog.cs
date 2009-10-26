using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BoC.Persistence;

namespace MvcTodo.Models.Entity
{
    public class Catalog : BaseEntity<int>
    {
        private List<Todo> _todos;
        public Catalog()
        {
            Todos = new List<Todo>();
        }

        public virtual string Name { get; set; }

        public virtual List<Todo> Todos
        {
            get { return _todos; }
            set { _todos = value; }
        }

    }
}
