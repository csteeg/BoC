using System;
using System.Collections.Generic;
using BoC.EventAggregator;
using Moq;
using Xunit;

namespace BoC.Tests.EventAggregator
{
    public class EventSubscriptionFixture
    {
        [Fact]
        public void Should_Throw_Exception_When_Passing_Null_Target_In_Action()
        {
            Assert.Throws<ArgumentException>(() => new EventSubscription<object>(new Mock<IDelegateReference>().Object, new Mock<IDelegateReference>().Object));
        }

        [Fact]
        public void Should_Throw_Exception_When_Passing_Null_Target_In_Filter()
        {
            var action = new Mock<IDelegateReference>();

            action.SetupGet(a => a.Target).Returns((Action<object>)delegate { });

            Assert.Throws<ArgumentException>(() => new EventSubscription<object>(action.Object, new Mock<IDelegateReference>().Object));
        }

        [Fact]
        public void Action_Should_Not_Be_Null()
        {
            var action = new Mock<IDelegateReference>();
            action.SetupGet(a => a.Target).Returns((Action<object>)delegate { });

            var filter = new Mock<IDelegateReference>();
            filter.SetupGet(a => a.Target).Returns((Predicate<object>)delegate { return true; });

            var subscription = new EventSubscription<object>(action.Object, filter.Object);

            Assert.NotNull(subscription.Action);
        }

        [Fact]
        public void Filter_Should_Not_be_Null()
        {
            var action = new Mock<IDelegateReference>();
            action.SetupGet(a => a.Target).Returns((Action<object>)delegate { });

            var filter = new Mock<IDelegateReference>();
            filter.SetupGet(a => a.Target).Returns((Predicate<object>)delegate { return true; });

            var subscription = new EventSubscription<object>(action.Object, filter.Object);

            Assert.NotNull(subscription.Filter);
        }

        [Fact]
        public void GetExecutionStrategy_Should_Return_Delegate_That_Executes_The_Filter_And_Then_The_Action()
        {
            var executedDelegates = new List<string>();

            var actionDelegate = new Mock<IDelegateReference>();

            actionDelegate.SetupGet(d => d.Target).Returns((Action<object>) delegate { executedDelegates.Add("Action"); });

            var filterDelegate = new Mock<IDelegateReference>();

            filterDelegate.SetupGet(d => d.Target).Returns((Predicate<object>) delegate { executedDelegates.Add("Filter"); return true; });

            var eventSubscription = new EventSubscription<object>(actionDelegate.Object, filterDelegate.Object);

            var publishAction = eventSubscription.GetExecutionStrategy();

            Assert.NotNull(publishAction);

            publishAction.Invoke(null);

            Assert.Equal(2, executedDelegates.Count);
            Assert.Equal("Filter", executedDelegates[0]);
            Assert.Equal("Action", executedDelegates[1]);
        }

        [Fact]
        public void GetExecutionStrategy_Should_Return_Null_If_Action_Is_Null()
        {
            var actionDelegate = new Mock<IDelegateReference>();

            actionDelegate.SetupGet(d => d.Target).Returns((Action<object>)delegate { });

            var filterDelegate = new Mock<IDelegateReference>();

            filterDelegate.SetupGet(d => d.Target).Returns((Predicate<object>)delegate { return true; });

            var eventSubscription = new EventSubscription<object>(actionDelegate.Object, filterDelegate.Object);

            var publishAction = eventSubscription.GetExecutionStrategy();

            Assert.NotNull(publishAction);

            actionDelegate.SetupGet(d => d.Target).Returns(null);

            publishAction = eventSubscription.GetExecutionStrategy();

            Assert.Null(publishAction);
        }

        [Fact]
        public void GetExecutionStrategy_Should_Return_Null_If_Filter_Is_Null()
        {
            var actionDelegate = new Mock<IDelegateReference>();

            actionDelegate.SetupGet(d => d.Target).Returns((Action<object>)delegate { });

            var filterDelegate = new Mock<IDelegateReference>();

            filterDelegate.SetupGet(d => d.Target).Returns((Predicate<object>)delegate { return true; });

            var eventSubscription = new EventSubscription<object>(actionDelegate.Object, filterDelegate.Object);

            var publishAction = eventSubscription.GetExecutionStrategy();

            Assert.NotNull(publishAction);

            filterDelegate.SetupGet(d => d.Target).Returns(null);

            publishAction = eventSubscription.GetExecutionStrategy();

            Assert.Null(publishAction);
        }

        [Fact]
        public void GetExecutionStrategy_Does_Not_Execute_Action_If_Filter_Returns_False()
        {
            bool actionExecuted = false;
            var actionDelegate = new Mock<IDelegateReference>();

            actionDelegate.SetupGet(d => d.Target).Returns((Action<int>)delegate { actionExecuted = true; });

            var filterDelegate = new Mock<IDelegateReference>();

            filterDelegate.SetupGet(d => d.Target).Returns((Predicate<int>)delegate { return false; });

            var eventSubscription = new EventSubscription<int>(actionDelegate.Object, filterDelegate.Object);

            var publishAction = eventSubscription.GetExecutionStrategy();

            publishAction.Invoke(new object[] { null });

            Assert.False(actionExecuted);
        }

        [Fact]
        public void Strategy_Should_Passe_Argument_To_Delegates()
        {
            string passedArgumentToAction = null;
            string passedArgumentToFilter = null;

            var actionDelegate = new Mock<IDelegateReference>();

            actionDelegate.SetupGet(d => d.Target).Returns((Action<string>)(obj => passedArgumentToAction = obj));

            var filterDelegate = new Mock<IDelegateReference>();

            filterDelegate.SetupGet(d => d.Target).Returns((Predicate<string>)(obj => { passedArgumentToFilter = obj; return true; }));

            var eventSubscription = new EventSubscription<string>(actionDelegate.Object, filterDelegate.Object);
            var publishAction = eventSubscription.GetExecutionStrategy();

            publishAction.Invoke(new[] { "TestString" });

            Assert.Equal("TestString", passedArgumentToAction);
            Assert.Equal("TestString", passedArgumentToFilter);
        }
    }
}