using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            return (from role in this.Query()
                    where role.RoleName == name
                    select role).FirstOrDefault();
        }

        virtual public Role[] FindByName(ICollection<String> names)
        {
            return (from role in this.Query()
                    where names.Contains(role.RoleName)
                    select role).ToArray();
        }

        #endregion

    }
}