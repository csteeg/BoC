using System;
using BoC.InversionOfControl;
using BoC.UnitOfWork;
using Moq;

namespace BoC.Tests
{
    public abstract class BaseIoCFixture : IDisposable
    {
        protected readonly Mock<IDependencyResolver> resolver;

        protected BaseIoCFixture()
        {
            resolver = new Mock<IDependencyResolver>();
            IoC.InitializeWith(resolver.Object);
        }
        
        protected Mock<T> SetupResolve<T>() where T : class
        {
            var repository = new Mock<T>();
            resolver.Setup(r => r.Resolve<T>()).Returns(repository.Object);
            return repository;
        }

        public virtual void Dispose()
        {
            IoC.Reset();
        }
    }
}