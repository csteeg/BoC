using System;
using System.Collections.Generic;
using System.Linq;
using BoC.EventAggregator;
using BoC.Events;
using BoC.Persistence;
using BoC.Services;
using BoC.Validation;
using Moq;
using Xunit;

namespace BoC.Tests.Services.BaseModelServiceFixtures
{
    public class FindFixture
    {
        private readonly Mock<IRepository<DummyModel>> repository;
        private readonly Mock<IModelValidator> validator;
        private readonly Mock<IEventAggregator> eventAggregator;
        private readonly Mock<BaseModelService<DummyModel>> service;
        private readonly DummyModel dummy1;
        private readonly DummyModel dummy2;
        private readonly DummyModel dummy3;
        private readonly DummyModel dummy4;
        private readonly DummyModel[] items;

        public FindFixture()
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


        [Fact]
        public void Find_Should_Perform_Query()
        {
            service.Object.Find(model => true);

            repository.Verify(r => r.Query(), Times.Once());
        }

        [Fact]
        public void Find_Should_Use_Linq_To_Filter_Queryable()
        {
            var found = service.Object.Find(model => (int)model.Id > 2);

            Assert.Equal(2, found.Count());
            Assert.Contains(dummy3, found);
            Assert.Contains(dummy4, found);
            Assert.DoesNotContain(dummy1, found);
            Assert.DoesNotContain(dummy2, found);
        }

        [Fact]
        public void Find_Should_Not_Return_IQueryable()
        {
            repository.Setup(r => r.Query()).Returns(items.AsQueryable());
            var found = service.Object.Find(model => true);

            Assert.False(found is IQueryable);
        }

    }
}