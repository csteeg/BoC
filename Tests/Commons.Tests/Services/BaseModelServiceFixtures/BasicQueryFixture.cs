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
    public class BasicQueryFixture : BaseModelServiceBaseFixture
    {
        [Fact]
        public void Get_Should_Get_Object_From_Repository()
        {
            service.Object.Get(1);

            repository.Verify(r => r.Get(1), Times.Once());
        }

        [Fact]
        public void InterfaceGet_Should_Redirect_To_Get()
        {
            ((IModelService) service.Object).Get(1);

            service.Verify(s => s.Get(1), Times.Once());
        }

        [Fact]
        public void ListAll_Should_Perform_Query()
        {
            service.Object.ListAll();

            repository.Verify(r => r.Query(), Times.Once());
        }

        [Fact]
        public void ListAll_Should_Convert_Queryable_To_List()
        {
            var list = service.Object.ListAll();

            Assert.IsAssignableFrom<IList<DummyModel>>(list);
            Assert.False(list is IQueryable);
        }

        [Fact]
        public void Count_Should_Use_Repository_Query()
        {
            service.Object.Count(model => true);

            repository.Verify(r => r.Query(), Times.Once());
        }

        [Fact]
        public void Count_Should_Use_Linq_To_Count()
        {
            var count = service.Object.Count(model => (int)model.Id > 2);

            Assert.Equal(2, count);
        }


   }
}