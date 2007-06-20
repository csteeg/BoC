using System;

namespace Steeg.Framework.Security
{
    public class RoleExistsException : UserServiceException
    {
        public RoleExistsException  () : base() { }
        public RoleExistsException  (String roleName) : this(roleName, null) { }
        public RoleExistsException(String roleName, Exception innerException) : base(string.Format("There already is a role with the name {0}", roleName), innerException) { }
        public RoleExistsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}