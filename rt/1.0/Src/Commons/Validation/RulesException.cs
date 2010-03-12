using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoC.Validation
{
    public class RulesException : Exception
    {
        public RulesException(IEnumerable<ErrorInfo> errors)
        {
            Errors = errors;
        }

        public RulesException(string propertyName, string errorMessage)
            : this(propertyName, errorMessage, null) { }

        public RulesException(string propertyName, string errorMessage, object onObject)
        {
            Errors = new[] { new ErrorInfo(propertyName, errorMessage, onObject) };
        }

        public IEnumerable<ErrorInfo> Errors { get; private set; }
    }
}
