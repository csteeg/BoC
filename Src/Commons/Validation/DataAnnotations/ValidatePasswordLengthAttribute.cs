using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BoC.Validation.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class MinLengthAttribute : ValidationAttribute
    {
        private readonly int minCharacters;
        private const string defaultErrorMessage = "'{0}' must be at least {1} characters long.";
        
        public MinLengthAttribute(int minCharacters)
            : base(defaultErrorMessage)
        {
            this.minCharacters = minCharacters;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString,
                                 name, minCharacters);
        }

        public override bool IsValid(object value)
        {
            var valueAsString = value as string;
            return (valueAsString != null && valueAsString.Length >= minCharacters);
        }
    }
}