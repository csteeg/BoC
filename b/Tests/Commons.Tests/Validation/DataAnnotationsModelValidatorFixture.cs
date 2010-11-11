using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BoC.Validation;
using Xunit;

namespace BoC.Tests.Validation
{
    public class DataAnnotationsModelValidatorFixture
    {
        [Fact]
        public void Validate_Should_Call_VadationAttributes_Validate()
        {
            var validatable = new ValidatableObject();
            var validator = new DataAnnotationsModelValidator();
            var result = validator.Validate(validatable);

            Assert.Empty(result);
        }

        [Fact]
        public void Validate_Should_Should_Return_Error_Info_For_Invalid_Property_Value()
        {
            var validatable = new ValidatableObject{Name = "IsError"};
            var validator = new DataAnnotationsModelValidator();
            var result = validator.Validate(validatable);

            Assert.Equal(1, result.Count());
            Assert.Equal(validatable, result.First().Object);
            Assert.Equal("Name", result.First().PropertyName);
            Assert.Equal("Name is invalid", result.First().ErrorMessage);
        }

        [Fact]
        public void Validate_Should_Should_Return_Error_Info_For_Multiple_Invalid_Values()
        {
            var validatable = new ValidatableObject { Name = "IsError", Name2 = "IsError" };
            var validator = new DataAnnotationsModelValidator();
            var result = validator.Validate(validatable);

            Assert.Equal(2, result.Count());
        }
    }

    public class ValidatableObject
    {
        [DummyValidator(ErrorMessage = "{0} is invalid")]
        public string Name { get; set; }

        [DummyValidator(ErrorMessage = "{0} is invalid")]
        public string Name2 { get; set; }
    }

    public class DummyValidator: ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return !"IsError".Equals(value);
        }

    }
}