namespace Steeg.Framework.Security
{
    using System;
    using System.Collections.Generic;
    using System.Security.Principal;

    public class Role
    {
        public Role() { }
        public Role(String roleName) 
        {
            this.roleName = roleName;
        }

        string roleName;
        public String RoleName
        {
            get
            {
                return this.roleName;
            }
        }

        IList<User> users = new List<User>();
        public IList<User> Users
        {
            get
            {
                return this.users;
            }
        }

        public static String[] ToString(IList<Role> roles)
        {
            if (roles == null)
                return null;

            String[] roleNames = new String[roles.Count];
            for (int i = 0; i < roles.Count; i++)
            {
                roleNames[i] = roles[i].RoleName;
            }
            return roleNames;
        }

        public override String ToString()
        {
            return this.RoleName;
        }

        public override bool Equals(object obj)
        {
 	        if (obj == null)
                return false;
            else if (obj is String)
                return ((String)obj).Equals(this.roleName, StringComparison.CurrentCultureIgnoreCase);
            else if (obj is Role)
                return ((Role)obj) == this;
            else
                return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            if (this.roleName == null)
                return 0;
            else
                return this.RoleName.ToLower().GetHashCode();
        }

        public static implicit operator String(Role role)
        {
            return role.RoleName;
        }

        public static bool operator == (Role role1, Role role2)
        {
            if (role1 == null)
                return (role2 == null);
            else if (role2 == null)
                return false;
            else if (role1.RoleName == null)
                return role2.roleName == null;
            else 
                return (role1.RoleName.Equals(role2.roleName, StringComparison.CurrentCultureIgnoreCase));
        }

        public static bool operator !=(Role role1, Role role2)
        {
            if (role1 == null)
                return (role2 != null);
            else if (role2 == null)
                return true;
            else if (role1.RoleName == null)
                return role2.roleName != null;
            else 
                return (!role1.RoleName.Equals(role2.roleName, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
