using System;
using System.Collections.Generic;
using System.Text;
using BoC.Persistence;
using BoC.Security.Model;

namespace BoC.Security.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        Role FindByName(String name);
        Role[] FindByName(ICollection<String> names);
    }
}