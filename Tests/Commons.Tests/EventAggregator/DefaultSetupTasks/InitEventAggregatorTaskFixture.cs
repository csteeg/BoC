using System;
using System.Collections.Generic;
using BoC.EventAggregator;
using BoC.EventAggregator.DefaultSetupTasks;
using Moq;
using Xunit;

namespace BoC.Tests.EventAggregator.DefaultSetupTasks
{
    public class InitEventAggregatorTaskFixture : BaseIoCFixture
    {
        [Fact]
        public void Execute_Should_Not_Register_EventAggregator_When_Already_Registered()
        {
            var obj = new InitEventAggregatorTask(resolver.Object);

            resolver.Setup(r => r.IsRegistered(typeof(IEventAggregator))).Returns(true).Verifiable();

            obj.Execute();

            resolver.Verify(r => r.IsRegistered(typeof(IEventAggregator)), Times.Once());
            //is only called once at the initializer
            resolver.Verify(r => r.RegisterSingleton(typeof(IEventAggregator), typeof(BoC.EventAggregator.EventAggregator)), Times.Never());
        }

        [Fact]
        public void Execute_Should_Register_EventAggregator_As_Singelton()
        {
            var obj = new InitEventAggregatorTask(resolver.Object);
            resolver.Setup(r => r.IsRegistered(typeof(IEventAggregator))).Returns(false);
            resolver.Setup(r => r.RegisterSingleton(typeof(IEventAggregator), typeof(BoC.EventAggregator.EventAggregator))).Verifiable();
            
            obj.Execute();
            //called once at the initializer & once by .execute()
            resolver.Verify(r => r.RegisterSingleton(typeof(IEventAggregator), typeof(BoC.EventAggregator.EventAggregator)), Times.Once());
        }
    }
}