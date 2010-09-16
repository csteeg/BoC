using System;
using BoC.InversionOfControl;
using BoC.Validation;
using BoC.Validation.DefaultSetupTasks;
using Moq;
using Xunit;

namespace BoC.Tests.Validation
{
    public class RegisterDefaultModelValidatorFixture
    {
        [Fact]
        public void Execute_Should_Register_Default_ModelValidator()
        {
            var resolver = new Mock<IDependencyResolver>();
            var task = new RegisterDefaultModelValidator(resolver.Object);

            task.Execute();

            try
            {
                resolver.Verify(r => r.RegisterType(typeof(IModelValidator), typeof(DataAnnotationsModelValidator)),
                                Times.Once());
            }
            catch (MockException)
            {
                //one of the verifies should've been called
                resolver.Verify(r => r.RegisterType<IModelValidator, DataAnnotationsModelValidator>(), Times.Once());
            }
        }

        [Fact]
        public void Execute_Should_Not_Register_Default_ModelValidator_If_Already_Registered()
        {
            var resolver = new Mock<IDependencyResolver>();
            resolver.Setup(r => r.IsRegistered(typeof (IModelValidator))).Returns(true);
            resolver.Setup(r => r.IsRegistered<IModelValidator>()).Returns(true);

            var task = new RegisterDefaultModelValidator(resolver.Object);

            task.Execute();

            resolver.Verify(r => r.RegisterType<IModelValidator, DataAnnotationsModelValidator>(), Times.Never());
            resolver.Verify(r => r.RegisterType(It.IsAny<Type>(), It.IsAny<Type>()), Times.Never());
        }
    }
}