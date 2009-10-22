using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BoC.Persistence.NHibernate;
using BoC.Security.Model;
using NHibernate;

namespace BoC.Security.Repositories.NHibernate
{
    public class RoleRepostitory: NHRepository<Role>, IRoleRepository
    {
        public RoleRepostitory(ISessionManager sessionManager) : base(sessionManager) { }

        #region IRoleDao Members

        virtual public Role FindByName(String name)
        {
            return this.FindOne(role => role.RoleName == name);
        }

        virtual public Role[] FindByName(ICollection<String> names)
        {
            return this.Query(role => names.Contains(role.RoleName));
        }

        #endregion

    }
}