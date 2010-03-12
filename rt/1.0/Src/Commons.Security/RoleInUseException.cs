using System;

namespace BoC.Security
{
    public class RoleInUseException : UserServiceException
    {
        public RoleInUseException(String roleName) : base("RoleName", string.Format("The role with the name {0} is currently assigned to one or more users", roleName)) { }
    }
}