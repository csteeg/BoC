using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoC.Validation
{
    public interface IModelValidator
    {
        IEnumerable<ErrorInfo> Validate(object model);
    }
}
