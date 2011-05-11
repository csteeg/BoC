using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o.Config;

namespace BoC.Persistence.db4o
{
    public interface IConfigurationExtender
    {
        void Configure(IEmbeddedConfiguration configuration);
    }
}
