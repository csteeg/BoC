using System;
using System.Transactions;
using BoC.Events;
using BoC.Persistence;
using Moq;
using Xunit;

namespace BoC.Tests.Services.BaseModelServiceFixtures
{
    public class DeleteFixture: BaseModelServiceBaseFixture
    {

        [Fact]
        public void Delete_Should_Delete_From_Repository()
        {
            eventAggregator.Setup(ev => ev.GetEvent<DeletingEvent<DummyModel>>()).Returns(new DeletingEvent<DummyModel>());
            eventAggregator.Setup(ev => ev.GetEvent<DeletedEvent<DummyModel>>()).Returns(new DeletedEvent<DummyModel>());

            service.Object.Delete(dummy1);

            repository.Verify(r => r.Delete(dummy1), Times.Once());
        }

        [Fact]
        public void Delete_Should_Trigger_DeletingEvent_Before_Deleting()
        {
            var deleteEvent = new Mock<DeletingEvent<DummyModel>>();

            eventAggregator.Setup(ev => ev.GetEvent<DeletingEvent<DummyModel>>()).Returns(deleteEvent.Object).Verifiable();
            eventAggregator.Setup(ev => ev.GetEvent<DeletedEvent<DummyModel>>()).Returns(new DeletedEvent<DummyModel>());

            repository.Setup(r => r.Delete(It.IsAny<DummyModel>())).Callback<DummyModel>(entity => entity.Saved = true);
            deleteEvent
                .Setup(ev => ev.Publish(It.IsAny<EventArgs<DummyModel>>()))
                .Callback<EventArgs<DummyModel>>(args =>
                                                     {
                                                         Assert.Equal(dummy1, args.Item);
                                                         Assert.False(args.Item.Saved); //ondeleting should be called before actual save
                                                     }).Verifiable();
            Assert.False(dummy1.Saved);
            service.Object.Delete(dummy1);

            eventAggregator.Verify();
            deleteEvent.Verify();
        }

        [Fact] public void Delete_Should_Not_Delete_If_DeletingEvent_Throws_Error()
        {
            var deletingEvent = new Mock<DeletingEvent<DummyModel>>();

            eventAggregator.Setup(ev => ev.GetEvent<DeletingEvent<DummyModel>>()).Returns(deletingEvent.Object).Verifiable();
            eventAggregator.Setup(ev => ev.GetEvent<DeletedEvent<DummyModel>>()).Returns(new DeletedEvent<DummyModel>());

            repository.Setup(r => r.Delete(It.IsAny<DummyModel>())).Callback<DummyModel>(entity => entity.Saved = true);
            deletingEvent
                .Setup(ev => ev.Publish(It.IsAny<EventArgs<DummyModel>>()))
                .Callback<EventArgs<DummyModel>>(args => { throw new Exception(); })
                .Verifiable();
            
            Assert.False(dummy1.Saved);
            Assert.Throws<Exception>(() => service.Object.Delete(dummy1));
            Assert.False(dummy1.Saved);
            eventAggregator.Verify();
            deletingEvent.Verify();
            repository.Verify(r => r.Delete(It.IsAny<DummyModel>()), Times.Never());
        }

        [Fact]
        public void Delete_Should_Happen_Within_Transaction()
        {
            var deletingEvent = new Mock<DeletingEvent<DummyModel>>();
            var deletedEvent = new Mock<DeletedEvent<DummyModel>>();
            eventAggregator.Setup(ev => ev.GetEvent<DeletingEvent<DummyModel>>()).Returns(deletingEvent.Object).Verifiable();
            eventAggregator
                .Setup(ev => ev.GetEvent<DeletedEvent<DummyModel>>())
                .Returns(deletedEvent.Object).Verifiable();

            repository.Setup(r => r.Delete(It.IsAny<DummyModel>()))
                .Callback<DummyModel>(entity => Assert.True(Transaction.Current.TransactionInformation.Status == TransactionStatus.Active));

            deletedEvent
                .Setup(ev => ev.Publish(It.IsAny<EventArgs<DummyModel>>()))
                .Callback(() => Assert.True(Transaction.Current.TransactionInformation.Status == TransactionStatus.Active))
                .Verifiable();
            deletingEvent
                .Setup(ev => ev.Publish(It.IsAny<EventArgs<DummyModel>>()))
                .Callback(() => Assert.True(Transaction.Current.TransactionInformation.Status == TransactionStatus.Active))
                .Verifiable();

            service.Object.Delete(dummy1);

            eventAggregator.Verify();
            deletingEvent.Verify();
            deletedEvent.Verify();
        }

    }
}