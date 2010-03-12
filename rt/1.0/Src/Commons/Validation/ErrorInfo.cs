using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoC.Validation
{
    public class ErrorInfo
    {
        public string PropertyName { get; private set; }
        public string ErrorMessage { get; private set; }
        public object Object { get; private set; }

        public ErrorInfo(string propertyName, string errorMessage, object onObject)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
            Object = onObject;
        }

        public ErrorInfo(string propertyName, string errorMessage)
            : this(propertyName, errorMessage, null)
        {
        }
    }
}
