using System;
using System.Transactions;
using BoC.Events;
using BoC.Persistence;
using BoC.Validation;
using Moq;
using Xunit;

namespace BoC.Tests.Services.BaseModelServiceFixtures
{
    public class ValidateEntityFixture : BaseModelServiceBaseFixture
    {
        [Fact]
        public void ValidateEntity_Should_Call_Validator()
        {
            service.Object.ValidateEntity(dummy1);

            validator.Verify(v => v.Validate(dummy1), Times.Once());
        }

        [Fact]
        public void ValidateEntity_Should_Throw_RulesException_If_Errors_Are_Noticed()
        {
            validator.Setup(v =>v.Validate(dummy1)).Returns(() => new[]{new ErrorInfo("Saved", "Is invalid boolean?", dummy1)}).Verifiable();
            Assert.Throws<RulesException>(() => service.Object.ValidateEntity(dummy1));

            validator.Verify();
        }
    }
}