using BoC.InversionOfControl;
using Moq;
using Xunit;

namespace BoC.UnitOfWork
{
    public class UnitOfWorkFixtture
    {
        [Fact]
        public void BeginUnitOfWork_Should_Ask_Resolver_For_IUnitOfWork()
        {
            var resolver = new Mock<IDependencyResolver>();
            IoC.InitializeWith(resolver.Object);

            UnitOfWork.BeginUnitOfWork();

            try
            {
                resolver.Verify(r => r.Resolve(typeof (IUnitOfWork)), Times.Once());
            }
            catch (MockException)
            {
                resolver.Verify(r => r.Resolve<IUnitOfWork>(), Times.Once());
            }
        }

        [Fact]
        public void BeginUnitOfWork_Should_Return_Dummy_UnitOfWork_If_No_IoC_Container_Is_Available()
        {
            IoC.Reset();
            var uow = UnitOfWork.BeginUnitOfWork();

            Assert.Equal("DummyUnitOfWork", uow.GetType().Name);
        }

        [Fact]
        public void BeginUnitOfWork_Should_Return_Dummy_UnitOfWork_If_No_IUnitOfWork_Registered()
        {
            var resolver = new Mock<IDependencyResolver>();
            IoC.InitializeWith(resolver.Object);

            using (var uow = UnitOfWork.BeginUnitOfWork())
            {
                Assert.Equal("DummyUnitOfWork", uow.GetType().Name);
            }
        }
    }
}