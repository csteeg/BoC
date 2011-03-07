using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Commons.Persistence.db4o.AutoIncrement
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AutoIncrementAttribute : Attribute
    {
    }
}
