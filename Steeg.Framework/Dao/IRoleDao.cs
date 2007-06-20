using System;
using System.Collections.Generic;
using System.Text;
using Steeg.Framework.Dao;
using Steeg.Framework.Security;
namespace Steeg.Framework.Dao
{
    public interface IRoleDao: IGenericDao<Role, long>
    {
        Role FindByName(String name);
        Role[] FindByName(ICollection<String> names);
    }
}
