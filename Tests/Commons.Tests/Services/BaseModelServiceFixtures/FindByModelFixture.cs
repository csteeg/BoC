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
    public class FindByModelFixture : BaseModelServiceBaseFixture
    {
        [Fact]
        public void FindByModelQuery_Should_Perform_Query()
        {
            service.Object.Find(ModelQuery.Where<DummyModel>(model => (int)model.Id > 2));
            ModelQuery<DummyModel> empty = null;
            service.Object.Find(empty);

            repository.Verify(r => r.Query(), Times.Exactly(2));
        }

        [Fact]
        public void FindByModelQuery_Should_Use_Linq_To_Filter_Queryable()
        {
            var found = service.Object.Find(ModelQuery.Where<DummyModel>(model => (int)model.Id > 2));

            Assert.Equal(2, found.Count());
            Assert.Contains(dummy3, found);
            Assert.Contains(dummy4, found);
            Assert.DoesNotContain(dummy1, found);
            Assert.DoesNotContain(dummy2, found);
        }

        [Fact]
        public void FindByModelQuery_Should_Use_Linq_To_Skip_Items()
        {
            var skip = new ModelQuery<DummyModel> {ItemsToSkip = 2};
            var found = service.Object.Find(skip);

            Assert.Equal(2, found.Count());
            Assert.Contains(dummy3, found);
            Assert.Contains(dummy4, found);
            Assert.DoesNotContain(dummy1, found);
            Assert.DoesNotContain(dummy2, found);
        }

        [Fact]
        public void FindByModelQuery_Should_Use_Linq_To_Take_Items()
        {
            var take = new ModelQuery<DummyModel> { ItemsToTake = 2 };
            var found = service.Object.Find(take);

            Assert.Equal(2, found.Count());
            Assert.DoesNotContain(dummy3, found);
            Assert.DoesNotContain(dummy4, found);
            Assert.Contains(dummy1, found);
            Assert.Contains(dummy2, found);
        }

        [Fact]
        public void FindByModelQuery_Should_Use_Linq_To_Skip_And_Take_Items()
        {
            var skip = new ModelQuery<DummyModel>().Skip(1).Take(1);
            var found = service.Object.Find(skip);

            Assert.Equal(1, found.Count());
            Assert.DoesNotContain(dummy3, found);
            Assert.DoesNotContain(dummy4, found);
            Assert.DoesNotContain(dummy1, found);
            Assert.Contains(dummy2, found);
        }

        [Fact]
        public void FindByModelQuery_Should_Use_Linq_To_Sort_Descending()
        {
            var sort = new ModelQuery<DummyModel>().OrderBy("Id Descending");
            var found = service.Object.Find(sort);

            Assert.Equal(4, found.Count());
            Assert.Equal(dummy4, found.First());
        }

        [Fact]
        public void FindByModelQuery_Should_Use_Linq_To_Sort()
        {
            var sort = new ModelQuery<DummyModel>().OrderBy("Id Ascending");
            var found = service.Object.Find(sort);

            Assert.Equal(4, found.Count());
            Assert.Equal(dummy1, found.First());
        }

        [Fact]
        public void FindByModelQuery_Should_Not_Return_IQueryable()
        {
            repository.Setup(r => r.Query()).Returns(items.AsQueryable());
            var found = service.Object.Find(ModelQuery.Where<DummyModel>(model => true));

            Assert.False(found is IQueryable);
        }

   }

}