using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BoC.Validation.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class CompareAttribute : ValidationAttribute//, IClientValidatable
    {
        private const string _defaultErrorMessage = "'{0}' and '{1}' do not match.";
        private readonly object _typeId = new object();

        public CompareAttribute(string confirmProperty)
            : base(_defaultErrorMessage)
        {
            ConfirmProperty = confirmProperty;
        }

        public string ConfirmProperty { get; private set; }

        public override object TypeId
        {
            get
            {
                return _typeId;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString,
                name, ConfirmProperty);
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var confirmValue = context.ObjectType.GetProperty(ConfirmProperty).GetValue(context.ObjectInstance, null);
            if (!Equals(value, confirmValue))
            {
                return new ValidationResult(FormatErrorMessage(context.DisplayName));
            }
            return null;
        }
        /*
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]{
                new ModelClientValidationEqualToRule(FormatErrorMessage(metadata.GetDisplayName()), ConfirmProperty)
            };
        }*/
    }

}