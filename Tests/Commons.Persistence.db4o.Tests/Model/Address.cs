using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoC.Persistence;

namespace Commons.Persistence.db4o.Tests.Model
{
    public class Address : BaseEntity<long>
    {
        public String Type { get; set; }
        public String Street { get; set; }
        public int HouseNumber { get; set; }

        public String ZipCode { get; set; }

    }
}
