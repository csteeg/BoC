using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoC.Persistence;

namespace Commons.Persistence.db4o.Tests.Model
{
    public class Person : BaseEntity<long>
    {
        public String FirstName { get; set; }

        public String LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        public Address HomeAddress { get; set; }
        
        public Address WorkAddress { get; set; }
    }
}
