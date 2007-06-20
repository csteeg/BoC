using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using NHibernate.Expression;
using Castle.Facilities.NHibernateIntegration;
using Steeg.Framework.Security;
using Steeg.Framework.Dao.NHibernateDao;

namespace Steeg.Framework.Dao.NHibernateDao
{
    public class RoleDao: NHibernateGenericDao<Role, Int64>, IRoleDao
    {
        public RoleDao(ISessionManager sessionManager) : base(sessionManager) { }

        #region IRoleDao Members

        virtual public Role FindByName(String name)
        {
            return this.FindOne("Name", name);
        }

        virtual public Role[] FindByName(ICollection<String> names)
        {
            return this.FindByProperty("Name", names);
        }

#endregion

    }
}
