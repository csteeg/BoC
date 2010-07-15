using System;
using System.Transactions;
using BoC.Events;
using BoC.Persistence;
using Moq;
using Xunit;

namespace BoC.Tests.Services.BaseModelServiceFixtures
{
    public class InsertFixture: BaseModelServiceBaseFixture
    {

        [Fact]
        public void Insert_Should_Save_To_Repository()
        {
            eventAggregator.Setup(ev => ev.GetEvent<InsertingEvent<DummyModel>>()).Returns(new InsertingEvent<DummyModel>());
            eventAggregator.Setup(ev => ev.GetEvent<InsertedEvent<DummyModel>>()).Returns(new InsertedEvent<DummyModel>());

            service.Object.Insert(dummy1);

            repository.Verify(r => r.Save(dummy1), Times.Once());
        }

        [Fact]
        public void Insert_Should_Return_Repository_Result()
        {
            eventAggregator.Setup(ev => ev.GetEvent<InsertingEvent<DummyModel>>()).Returns(new InsertingEvent<DummyModel>());
            eventAggregator.Setup(ev => ev.GetEvent<InsertedEvent<DummyModel>>()).Returns(new InsertedEvent<DummyModel>());
            
            repository.Setup(r => r.Save(It.IsAny<DummyModel>())).Callback<DummyModel>(entity => entity.Saved = true).Returns(dummy1);

            Assert.False(dummy1.Saved);
            var result = service.Object.Insert(dummy1);

            Assert.True(result.Saved);
        }

        [Fact]
        public void Insert_Should_Trigger_InsertingEvent_Before_Inserting()
        {
            var insertEvent = new Mock<InsertingEvent<DummyModel>>();
            
            eventAggregator.Setup(ev => ev.GetEvent<InsertingEvent<DummyModel>>()).Returns(insertEvent.Object).Verifiable();
            eventAggregator.Setup(ev => ev.GetEvent<InsertedEvent<DummyModel>>()).Returns(new InsertedEvent<DummyModel>());

            repository.Setup(r => r.Save(It.IsAny<DummyModel>())).Callback<DummyModel>(entity => entity.Saved = true);
            insertEvent
                .Setup(ev => ev.Publish(It.IsAny<EventArgs<DummyModel>>()))
                .Callback<EventArgs<DummyModel>>(args =>
                                                     {
                                                         Assert.Equal(dummy1, args.Item);
                                                         Assert.False(args.Item.Saved); //oninserting should be called before actual save
                                                     }).Verifiable();
            Assert.False(dummy1.Saved);
            service.Object.Insert(dummy1);

            eventAggregator.Verify();
            insertEvent.Verify();
        }

        [Fact]
        public void Insert_Should_Not_Insert_If_InsertingEvent_Throws_Error()
        {
            var insertEvent = new Mock<InsertingEvent<DummyModel>>();

            eventAggregator.Setup(ev => ev.GetEvent<InsertingEvent<DummyModel>>()).Returns(insertEvent.Object).Verifiable();
            eventAggregator.Setup(ev => ev.GetEvent<InsertedEvent<DummyModel>>()).Returns(new InsertedEvent<DummyModel>());

            repository.Setup(r => r.Save(It.IsAny<DummyModel>())).Callback<DummyModel>(entity => entity.Saved = true);
            insertEvent
                .Setup(ev => ev.Publish(It.IsAny<EventArgs<DummyModel>>()))
                .Callback<EventArgs<DummyModel>>(args => { throw new Exception(); })
                .Verifiable();
            
            Assert.False(dummy1.Saved);
            Assert.Throws<Exception>(() => service.Object.Insert(dummy1));
            Assert.False(dummy1.Saved);
            eventAggregator.Verify();
            insertEvent.Verify();
            repository.Verify(r => r.Save(It.IsAny<DummyModel>()), Times.Never());
        }

        [Fact]
        public void Insert_Should_Happen_Within_Transaction()
        {
            var insertingEvent = new Mock<InsertingEvent<DummyModel>>();
            var insertedEvent = new Mock<InsertedEvent<DummyModel>>();
            eventAggregator.Setup(ev => ev.GetEvent<InsertingEvent<DummyModel>>()).Returns(insertingEvent.Object).Verifiable();
            eventAggregator
                .Setup(ev => ev.GetEvent<InsertedEvent<DummyModel>>())
                .Returns(insertedEvent.Object).Verifiable();

            repository.Setup(r => r.Save(It.IsAny<DummyModel>()))
                .Callback<DummyModel>(entity => Assert.True(Transaction.Current.TransactionInformation.Status == TransactionStatus.Active));
            
            insertedEvent
                .Setup(ev => ev.Publish(It.IsAny<EventArgs<DummyModel>>()))
                .Callback(() => Assert.True(Transaction.Current.TransactionInformation.Status == TransactionStatus.Active))
                .Verifiable();
            insertingEvent
                .Setup(ev => ev.Publish(It.IsAny<EventArgs<DummyModel>>()))
                .Callback(() => Assert.True(Transaction.Current.TransactionInformation.Status == TransactionStatus.Active))
                .Verifiable();

            service.Object.Insert(dummy1);

            eventAggregator.Verify();
            insertingEvent.Verify();
            insertedEvent.Verify();
        }
    }
}