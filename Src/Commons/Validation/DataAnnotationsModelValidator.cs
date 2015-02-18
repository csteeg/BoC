using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using BoC.Profiling;

namespace BoC.Validation
{
    public class DataAnnotationsModelValidator: IModelValidator
    {
        public IEnumerable<ErrorInfo> Validate(object instance)
        {
            using (Profiler.StartContext("DataAnnotationsModelValidator.Validate({0})", instance))
                return (from prop in TypeDescriptor.GetProperties(instance).Cast<PropertyDescriptor>()
                   from attribute in prop.Attributes.OfType<ValidationAttribute>()
                   where !attribute.IsValid(prop.GetValue(instance))
                   select new ErrorInfo(prop.Name, attribute.FormatErrorMessage(prop.DisplayName), instance)).ToList();
        }
    }
}
