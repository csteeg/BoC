using BoC.EventAggregator;
using Xunit;

namespace BoC.Tests.EventAggregator
{
    public class EventAggregatorFixture
    {
        private readonly BoC.EventAggregator.EventAggregator _eventAggregator;

        public EventAggregatorFixture()
        {
            _eventAggregator = new BoC.EventAggregator.EventAggregator();
        }

        [Fact]
        public void Get_Should_Return_Same_Instance_Of_Same_Event_Type()
        {
            var instance1 = _eventAggregator.GetEvent<SomeEvent>();
            var instance2 = _eventAggregator.GetEvent<SomeEvent>();

            Assert.Same(instance2, instance1);

            instance1.Tag = "Should be same";

            Assert.Equal("Should be same", instance2.Tag);
        }
    }

    public class SomeEvent: BaseEvent
    {
        public string Tag { get; set; }
    }
}