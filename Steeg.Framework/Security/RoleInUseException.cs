using System;

namespace Steeg.Framework.Security
{
    public class RoleInUseException : UserServiceException
    {
        public RoleInUseException  () : base() { }
        public RoleInUseException(String roleName) : this(roleName, null) { }
        public RoleInUseException(String roleName, Exception innerException) : base(string.Format("The role with the name {0} is currently assigned to one or more users", roleName), innerException) { }
        public RoleInUseException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}