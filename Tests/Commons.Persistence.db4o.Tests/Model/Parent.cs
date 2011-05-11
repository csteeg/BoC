using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoC.Persistence;

namespace Commons.Persistence.db4o.Tests.Model
{
    public class Parent : BaseEntity<long>
    {
        public Child Child { get; set; }
    }
}
