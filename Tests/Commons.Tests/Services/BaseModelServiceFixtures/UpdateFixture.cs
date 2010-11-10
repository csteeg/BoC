using System;
using System.Transactions;
using BoC.Events;
using BoC.Persistence;
using Moq;
using Xunit;

namespace BoC.Tests.Services.BaseModelServiceFixtures
{
    public class UpdateFixture: BaseModelServiceBaseFixture
    {

        [Fact]
        public void Update_Should_SaveOrUpdate_To_Repository()
        {
            eventAggregator.Setup(ev => ev.GetEvent<UpdatingEvent<DummyModel>>()).Returns(new UpdatingEvent<DummyModel>());
            eventAggregator.Setup(ev => ev.GetEvent<UpdatedEvent<DummyModel>>()).Returns(new UpdatedEvent<DummyModel>());

            service.Object.SaveOrUpdate(dummy1);

            repository.Verify(r => r.SaveOrUpdate(dummy1), Times.Once());
        }

        [Fact]
        public void Update_Should_Trigger_UpdatingEvent_Before_Updating()
        {
            var updateEvent = new Mock<UpdatingEvent<DummyModel>>();

            eventAggregator.Setup(ev => ev.GetEvent<UpdatingEvent<DummyModel>>()).Returns(updateEvent.Object).Verifiable();
            eventAggregator.Setup(ev => ev.GetEvent<UpdatedEvent<DummyModel>>()).Returns(new UpdatedEvent<DummyModel>());

            repository.Setup(r => r.SaveOrUpdate(It.IsAny<DummyModel>())).Callback<DummyModel>(entity => entity.Saved = true);
            updateEvent
                .Setup(ev => ev.Publish(It.IsAny<EventArgs<DummyModel>>()))
                .Callback<EventArgs<DummyModel>>(args =>
                                                     {
                                                         Assert.Equal(dummy1, args.Item);
                                                         Assert.False(args.Item.Saved); //onUpdating should be called before actual save
                                                     }).Verifiable();
            Assert.False(dummy1.Saved);
            service.Object.SaveOrUpdate(dummy1);

            eventAggregator.Verify();
            updateEvent.Verify();
        }

        [Fact] public void Update_Should_Not_Update_If_UpdatingEvent_Throws_Error()
        {
            var UpdatingEvent = new Mock<UpdatingEvent<DummyModel>>();

            eventAggregator.Setup(ev => ev.GetEvent<UpdatingEvent<DummyModel>>()).Returns(UpdatingEvent.Object).Verifiable();
            eventAggregator.Setup(ev => ev.GetEvent<UpdatedEvent<DummyModel>>()).Returns(new UpdatedEvent<DummyModel>());

            repository.Setup(r => r.SaveOrUpdate(It.IsAny<DummyModel>())).Callback<DummyModel>(entity => entity.Saved = true);
            UpdatingEvent
                .Setup(ev => ev.Publish(It.IsAny<EventArgs<DummyModel>>()))
                .Callback<EventArgs<DummyModel>>(args => { throw new Exception(); })
                .Verifiable();
            
            Assert.False(dummy1.Saved);
            Assert.Throws<Exception>(() => service.Object.SaveOrUpdate(dummy1));
            Assert.False(dummy1.Saved);
            eventAggregator.Verify();
            UpdatingEvent.Verify();
            repository.Verify(r => r.Update(It.IsAny<DummyModel>()), Times.Never());
        }

        [Fact]
        public void Update_Should_Happen_Within_Transaction()
        {
            var updatingEvent = new Mock<UpdatingEvent<DummyModel>>();
            var updatedEvent = new Mock<UpdatedEvent<DummyModel>>();
            eventAggregator.Setup(ev => ev.GetEvent<UpdatingEvent<DummyModel>>()).Returns(updatingEvent.Object).Verifiable();
            eventAggregator
                .Setup(ev => ev.GetEvent<UpdatedEvent<DummyModel>>())
                .Returns(updatedEvent.Object).Verifiable();

            repository.Setup(r => r.SaveOrUpdate(It.IsAny<DummyModel>()))
                .Callback<DummyModel>(entity => Assert.True(Transaction.Current.TransactionInformation.Status == TransactionStatus.Active));

            updatedEvent
                .Setup(ev => ev.Publish(It.IsAny<EventArgs<DummyModel>>()))
                .Callback(() => Assert.True(Transaction.Current.TransactionInformation.Status == TransactionStatus.Active))
                .Verifiable();
            updatingEvent
                .Setup(ev => ev.Publish(It.IsAny<EventArgs<DummyModel>>()))
                .Callback(() => Assert.True(Transaction.Current.TransactionInformation.Status == TransactionStatus.Active))
                .Verifiable();

            service.Object.SaveOrUpdate(dummy1);

            eventAggregator.Verify();
            updatingEvent.Verify();
            updatedEvent.Verify();
        }

    }
}