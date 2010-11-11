using System.Linq;
using BoC.EventAggregator;
using BoC.Persistence;
using BoC.Services;
using BoC.Validation;
using Moq;

namespace BoC.Tests.Services.BaseModelServiceFixtures
{
    public abstract class BaseModelServiceBaseFixture
    {
        protected readonly Mock<IRepository<DummyModel>> repository;
        protected readonly Mock<IModelValidator> validator;
        protected readonly Mock<IEventAggregator> eventAggregator;
        protected readonly Mock<BaseModelService<DummyModel>> service;
        protected readonly DummyModel dummy1;
        protected readonly DummyModel dummy2;
        protected readonly DummyModel dummy3;
        protected readonly DummyModel dummy4;
        protected readonly DummyModel[] items;

        public BaseModelServiceBaseFixture()
        {
            repository = new Mock<IRepository<DummyModel>>();
            validator = new Mock<IModelValidator>();
            eventAggregator = new Mock<IEventAggregator>();
            service = new Mock<BaseModelService<DummyModel>>(validator.Object, eventAggregator.Object, repository.Object);
            service.CallBase = true;

            dummy1 = new DummyModel() { Id = 1 };
            dummy2 = new DummyModel() { Id = 2 };
            dummy3 = new DummyModel() { Id = 3 };
            dummy4 = new DummyModel() { Id = 4 };
            items = new[]
                            {
                                dummy1,
                                dummy2,
                                dummy3,
                                dummy4
                            };
            repository.Setup(r => r.Query()).Returns(items.AsQueryable());

        }
    }
}