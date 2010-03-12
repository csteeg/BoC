using System;
using System.Collections.Generic;
using BoC.Validation;

namespace BoC.Security
{
    public class UserServiceException : RulesException
    {
        public UserServiceException(IEnumerable<ErrorInfo> errors) : base(errors) {}
        public UserServiceException(string propertyName, string errorMessage) : base(propertyName, errorMessage) {}
        public UserServiceException(string propertyName, string errorMessage, object onObject) : base(propertyName, errorMessage, onObject) {}
    }
}