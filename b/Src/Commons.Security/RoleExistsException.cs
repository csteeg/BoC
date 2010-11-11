using System;

namespace BoC.Security
{
    public class RoleExistsException : UserServiceException
    {
        public RoleExistsException(String roleName) : base("RoleName", string.Format("There already is a role with the name {0}", roleName)) { }
    }
}